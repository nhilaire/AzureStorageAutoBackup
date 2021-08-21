using AzureStorageAutoBackup.Files;
using AzureStorageAutoBackup.State;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public FileUploader(IStorageReader storageReader, IStorageCommand storageCommand, IFilesState filesState, ILogger<FileUploader> logger, Md5 md5)
        {
            _storageReader = storageReader;
            _storageCommand = storageCommand;
            _filesState = filesState;
            _logger = logger;
            _md5 = md5;
        }

        public async Task<List<FileItem>> UploadIfNeeded(List<FileItem> files)
        {
            _logger.LogTrace("Reading actual storage ...");
            var existingFiles = await _storageReader.BrowseStorage();

            if (files.Count > 0)
            {
                var missingDirectories = ComputeMissingDirectories(files, existingFiles);

                _logger.LogTrace($"Nb directories to create in storage : {missingDirectories.Count}");
                await _storageCommand.CreateDirectories(missingDirectories);

                var alreadyBackupedFiles = _filesState.CompletedFiles;

                foreach (var file in files)
                {
                    file.Checksum = _md5.CalculateMD5(file.Path);
                    if (!file.ExistsIn(alreadyBackupedFiles))
                    {
                        if (!file.ExistsIn(existingFiles))
                        {
                            await _storageCommand.UploadToStorage(file);
                        }
                        else
                        {
                            await _filesState.Save(file);
                        }
                    }
                }
            }

            return existingFiles;
        }

        public async Task CleanFilesAndDirectories(List<FileItem> filesInStorage, List<FileItem> fileListToBackup)
        {
            var filesToDeleteInStorage = filesInStorage.Select(x => x.Path).Except(fileListToBackup.Select(x => x.Path)).ToList();
            _logger.LogTrace($"Nb files to delete in storage : {filesToDeleteInStorage.Count}");
            await _storageCommand.DeleteFiles(filesToDeleteInStorage);

            await _storageCommand.DeleteEmptyDirectoriesIfExist();
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
