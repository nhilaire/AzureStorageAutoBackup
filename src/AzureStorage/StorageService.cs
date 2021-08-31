using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using AzureStorageAutoBackup.State;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.AzureStorage
{
    public class StorageService : IStorageReader, IStorageCommand
    {
        private const string customChecksumMetadataName = "customchecksum";
        private readonly AppConfiguration _appConfiguration;
        private readonly ShareClient _shareClient;
        private readonly IFilesState _filesState;
        private readonly ILogger<StorageService> _logger;
        private readonly ApplicationStat _applicationStat;

        public StorageService(IOptions<AppConfiguration> appConfiguration, IFilesState filesState, ILogger<StorageService> logger, ApplicationStat applicationStat)
        {
            _appConfiguration = appConfiguration.Value;
            _shareClient = new ShareClient(_appConfiguration.ConnectionString, _appConfiguration.ShareReference);
            _filesState = filesState;
            _logger = logger;
            _applicationStat = applicationStat;
        }

        public async Task CreateDirectories(List<string> directories, CancellationTokenSource cancellationToken)
        {
            if (directories.Count == 0)
            {
                return;
            }

            await _shareClient.CreateIfNotExistsAsync();
            if (await _shareClient.ExistsAsync())
            {
                var baseDirectory = _shareClient.GetDirectoryClient(_appConfiguration.DestinationPathInAzure);
                await baseDirectory.CreateIfNotExistsAsync();
                if (await baseDirectory.ExistsAsync())
                {
                    foreach (var directory in directories.OrderBy(x => x.Length))
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        var path = ToAzureNameWithoutBasePath(directory);
                        var subDirectory = baseDirectory.GetSubdirectoryClient(path);
                        await subDirectory.CreateIfNotExistsAsync();
                    }
                }
            }
        }

        public async Task UploadToStorage(FileItem file, CancellationTokenSource cancellationToken)
        {
            var baseDirectory = _shareClient.GetDirectoryClient(_appConfiguration.DestinationPathInAzure);
            var azurePath = ToAzureNameWithoutBasePath(file.Path);
            var fileClient = baseDirectory.GetFileClient(azurePath);

            using (var stream = File.OpenRead(file.Path))
            {
                fileClient.Create(stream.Length);
                try
                {
                    await UploadFileAsync(cancellationToken, fileClient, stream, $"{file.Path} : {file.State}");
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
            var metadata = new Dictionary<string, string>
            {
                {customChecksumMetadataName, file.Checksum}
            };
            await fileClient.SetMetadataAsync(metadata);
            await _filesState.Save(file);
        }

        private async Task UploadFileAsync(CancellationTokenSource cancellationToken, ShareFileClient fileClient, Stream stream, string fileNameAndState)
        {
            if (stream.Length == 0)
            {
                return;
            }
            const int uploadLimit = 4 * 1024 * 1024;

            long index = 0;
            var progress = new Progress<long>(bytesUploaded =>
            {
                double percentComplete = (double)(bytesUploaded + index) / stream.Length;
                _logger.LogTrace($"\rUpload \t\t{fileNameAndState}\t" + percentComplete.ToString("P"));
            });

            stream.Seek(0, SeekOrigin.Begin);

            if (stream.Length <= uploadLimit)
            {
                await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream, null, progress, cancellationToken: cancellationToken.Token);
                return;
            }

            int bytesRead;
            byte[] buffer = new byte[uploadLimit];

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                using var ms = new MemoryStream(buffer, 0, bytesRead);
                await fileClient.UploadRangeAsync(ShareFileRangeWriteType.Update, new HttpRange(index, ms.Length), ms, null, progress, cancellationToken: cancellationToken.Token);
                index += ms.Length;
            }
        }

        public async Task DeleteFiles(List<string> filesToDelete, CancellationTokenSource cancellationTokenSource)
        {
            if (filesToDelete.Count == 0)
            {
                return;
            }

            var baseDirectory = _shareClient.GetDirectoryClient(_appConfiguration.DestinationPathInAzure);
            foreach (var file in filesToDelete)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                var path = ToAzureNameWithoutBasePath(file);
                _applicationStat.DeleteFilesCount++;
                _logger.LogTrace($"Delete from azure storage {path}");
                await baseDirectory.DeleteFileAsync(path);
                await _filesState.Delete(file);
            }
        }

        public async Task DeleteEmptyDirectoriesIfExist(CancellationTokenSource cancellationTokenSource)
        {
            var baseDirectory = _shareClient.GetDirectoryClient(_appConfiguration.DestinationPathInAzure);
            await DeleteEmptyDirectoriesRecursive(baseDirectory, cancellationTokenSource);
        }

        private async Task DeleteEmptyDirectoriesRecursive(ShareDirectoryClient directoryClient, CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            await foreach (var item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (item.IsDirectory)
                {
                    var subDirectoryClient = directoryClient.GetSubdirectoryClient(item.Name);
                    await DeleteEmptyDirectoriesRecursive(subDirectoryClient, cancellationTokenSource);
                }
            }

            var count = 0;
            await foreach (var item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                count++;
            }
            if (count == 0)
            {
                try
                {
                    await directoryClient.DeleteAsync();
                    _logger.LogTrace($"Deleting {directoryClient.Path}");
                }
                catch (Exception)
                {
                    // no need to handle exception
                }
            }
        }

        public async Task<List<FileItem>> BrowseStorage(CancellationTokenSource cancellationToken)
        {
            var result = new List<FileItem>();
            await _shareClient.CreateIfNotExistsAsync();
            if (await _shareClient.ExistsAsync())
            {
                var baseDirectory = _shareClient.GetDirectoryClient(_appConfiguration.DestinationPathInAzure);
                await baseDirectory.CreateIfNotExistsAsync();
                if (await baseDirectory.ExistsAsync())
                {
                    await RecursiveFindFile(baseDirectory, result, cancellationToken);
                }
            }
            return result;
        }

        private async Task RecursiveFindFile(ShareDirectoryClient directoryClient, List<FileItem> result, CancellationTokenSource cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await foreach (var item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (item.IsDirectory)
                {
                    var subDirectoryClient = directoryClient.GetSubdirectoryClient(item.Name);
                    await RecursiveFindFile(subDirectoryClient, result, cancellationToken);
                }
                else
                {
                    var fileClient = directoryClient.GetFileClient(item.Name);
                    var properties = await fileClient.GetPropertiesAsync();
                    properties.Value.Metadata.TryGetValue(customChecksumMetadataName, out var checksum);
                    result.Add(new FileItem
                    {
                        Path = ToWindowsName(directoryClient.Path, item.Name),
                        Checksum = checksum
                    });
                }
            }
        }

        private static string ToAzureNameWithoutBasePath(string path)
        {
            return path.Replace(":\\", "/").Replace('\\', '/');
        }

        private string ToWindowsName(string directoryName, string itemName)
        {
            if (directoryName.StartsWith(_appConfiguration.DestinationPathInAzure))
            {
                directoryName = directoryName[_appConfiguration.DestinationPathInAzure.Length..];
            }
            if (directoryName.StartsWith("/"))
            {
                directoryName = directoryName[1..];
            }
            directoryName = directoryName.Replace("/", "\\");
            directoryName = directoryName.Insert(1, ":");

            return $"{directoryName}\\{itemName}";
        }
    }
}
