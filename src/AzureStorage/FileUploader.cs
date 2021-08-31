using AzureStorageAutoBackup.Files;
using AzureStorageAutoBackup.State;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.AzureStorage
{
    public class FileUploader
    {
        private readonly IStorageReader _storageReader;
        private readonly IStorageCommand _storageCommand;
        private readonly IFilesState _filesState;
        private readonly ILogger<FileUploader> _logger;
        private readonly Md5 _md5;
        private readonly ApplicationStat _applicationStat;

        public FileUploader(IStorageReader storageReader, IStorageCommand storageCommand, IFilesState filesState, ILogger<FileUploader> logger, Md5 md5, ApplicationStat applicationStat)
        {
            _storageReader = storageReader;
            _storageCommand = storageCommand;
            _filesState = filesState;
            _logger = logger;
            _md5 = md5;
            _applicationStat = applicationStat;
        }

        public async Task<List<FileItem>> UploadIfNeeded(List<FileItem> files, CancellationTokenSource cancellationToken)
        {
            _logger.LogTrace("Reading actual storage ...");
            var existingFiles = await _storageReader.BrowseStorage(cancellationToken);

            if (files.Count > 0)
            {
                var missingDirectories = ComputeMissingDirectories(files, existingFiles);
                _applicationStat.MissingDirectoriesCount = missingDirectories.Count;

                _logger.LogTrace($"Nb directories to create in storage : {_applicationStat.MissingDirectoriesCount}");
                await _storageCommand.CreateDirectories(missingDirectories, cancellationToken);

                var alreadyBackupedFiles = _filesState.CompletedFiles;

                foreach (var file in files)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return existingFiles;
                    }

                    try
                    {
                        file.Checksum = _md5.CalculateMD5(file.Path);
                        if (file.ExistsIn(alreadyBackupedFiles))
                        {
                            _applicationStat.AlreadyBackupedCount++;
                        }
                        else
                        {
                            if (file.ExistsIn(existingFiles))
                            {
                                _applicationStat.AlreadyBackupedCount++;
                                await _filesState.Save(file);
                            }
                            else
                            {
                                if (file.State == FileState.New)
                                {
                                    _applicationStat.NewFilesCount++;
                                }
                                else
                                {
                                    _applicationStat.UpdateFilesCount++;
                                }

                                await _storageCommand.UploadToStorage(file, cancellationToken);
                            }
                        }
                    }
                    catch (IOException)
                    {
                        _applicationStat.FilesInErrors.Add(file.Path);
                    }
                }
            }

            return existingFiles;
        }

        public async Task CleanFilesAndDirectories(List<FileItem> filesInStorage, List<FileItem> fileListToBackup, CancellationTokenSource cancellationToken)
        {
            var filesToDeleteInStorage = filesInStorage.Select(x => x.Path).Except(fileListToBackup.Select(x => x.Path)).ToList();
            _logger.LogTrace($"Nb files to delete in storage : {filesToDeleteInStorage.Count}");
            await _storageCommand.DeleteFiles(filesToDeleteInStorage, cancellationToken);

            await _storageCommand.DeleteEmptyDirectoriesIfExist(cancellationToken);
        }

        private static List<string> ComputeMissingDirectories(List<FileItem> files, List<FileItem> existingFiles)
        {
            var allDistinctDirectories = files.Select(x => GetDirectoriesWithParents(x.Path)).SelectMany(x => x).Distinct().ToList();
            var existingDistinctDirectories = existingFiles.Select(x => GetDirectoriesWithParents(x.Path)).SelectMany(x => x).Distinct().ToList();
            return allDistinctDirectories.Except(existingDistinctDirectories).ToList();
        }

        private static List<string> GetDirectoriesWithParents(string filePath)
        {
            var result = new List<string>();

            var path = Path.GetDirectoryName(filePath);
            var directoryInfo = new DirectoryInfo(path);

            result.Add(path);
            while (directoryInfo.Parent != null)
            {
                directoryInfo = directoryInfo.Parent;
                result.Add(directoryInfo.FullName);
            }

            return result;
        }
    }
}
