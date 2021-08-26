using AzureStorageAutoBackup.AzureStorage;
using AzureStorageAutoBackup.Files;
using AzureStorageAutoBackup.State;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup
{
    public class Backuper
    {
        private readonly FilesBuilder _filesBuilder;
        private readonly IFilesState _filesState;
        private readonly FileUploader _fileUploader;
        private readonly ILogger<Backuper> _logger;
        private readonly ApplicationStat _applicationStat;

        public Backuper(FilesBuilder filesBuilder, IFilesState filesState, FileUploader fileUploader, ILogger<Backuper> logger, ApplicationStat applicationStat)
        {
            _filesBuilder = filesBuilder;
            _filesState = filesState;
            _fileUploader = fileUploader;
            _logger = logger;
            _applicationStat = applicationStat;
        }

        public async Task Run()
        {
            try
            {
                await _filesState.Load();

                var fileListToBackup = await _filesBuilder.GetFileListToBackup();
                _applicationStat.FilesToBackupCount = fileListToBackup.Count;

                _logger.LogTrace($"Nb files to backup : {_applicationStat.FilesToBackupCount}");

                fileListToBackup = RandomizeOrder(fileListToBackup);

                var filesInStorage = await _fileUploader.UploadIfNeeded(fileListToBackup);
                await _fileUploader.CleanFilesAndDirectories(filesInStorage, fileListToBackup);

                _logger.LogTrace("End ...");

                _logger.LogTrace(@$"Nb files already backuped : {_applicationStat.AlreadyBackupedCount}
Nb new files : {_applicationStat.NewFilesCount}
Nb updated files : {_applicationStat.UpdateFilesCount}
Nb deleted files : {_applicationStat.DeleteFilesCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during the backup");
            }
        }

        private List<FileItem> RandomizeOrder(List<FileItem> fileListToBackup)
        {
            return fileListToBackup.OrderBy(_ => Guid.NewGuid()).Select(x => x).ToList();
        }
    }
}
