using AzureStorageAutoBackup.AzureStorage;
using AzureStorageAutoBackup.Files;
using AzureStorageAutoBackup.State;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup
{
    public class Backuper
    {
        private readonly FilesBuilder _filesBuilder;
        private readonly IFilesState _filesState;
        private readonly FileUploader _fileUploader;
        private readonly ILogger<Backuper> _logger;

        public Backuper(FilesBuilder filesBuilder, IFilesState filesState, FileUploader fileUploader, ILogger<Backuper> logger)
        {
            _filesBuilder = filesBuilder;
            _filesState = filesState;
            _fileUploader = fileUploader;
            _logger = logger;
        }

        public async Task Run()
        {
            try
            {
                var fileListToBackup = await _filesBuilder.GetFileListToBackup();

                _logger.LogTrace($"Nb files to backup : {fileListToBackup.Count}");

                await _filesState.Load();
                var alreadyBackupedFiles = _filesState.CompletedFiles;

                var effectivesFilesToBackup = GetFilesToBackup(fileListToBackup, alreadyBackupedFiles);

                _logger.LogTrace($"Nb files to upload : {effectivesFilesToBackup.Count}");

                var filesInStorage = await _fileUploader.UploadIfNeeded(effectivesFilesToBackup);
                await _fileUploader.CleanFilesAndDirectories(filesInStorage, fileListToBackup);

                _logger.LogTrace("End ...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during the backup");
            }
        }

        private List<FileItem> GetFilesToBackup(List<FileItem> filesToBackup, List<FileItem> alreadyBackupedFiles)
        {
            return FileItemComparer.GetFilesIntersection(filesToBackup, alreadyBackupedFiles);
        }
    }
}
