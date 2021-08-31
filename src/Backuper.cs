using AzureStorageAutoBackup.AzureStorage;
using AzureStorageAutoBackup.Files;
using AzureStorageAutoBackup.Mail;
using AzureStorageAutoBackup.State;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly IMailSender _mailSender;

        public Backuper(FilesBuilder filesBuilder, IFilesState filesState, FileUploader fileUploader, ILogger<Backuper> logger, ApplicationStat applicationStat, IMailSender mailSender)
        {
            _filesBuilder = filesBuilder;
            _filesState = filesState;
            _fileUploader = fileUploader;
            _logger = logger;
            _applicationStat = applicationStat;
            _mailSender = mailSender;
        }

        public async Task Run(CancellationTokenSource cancellationToken)
        {
            try
            {
                await _filesState.Load();

                var fileListToBackup = await _filesBuilder.GetFileListToBackup();
                _applicationStat.FilesToBackupCount = fileListToBackup.Count;

                _logger.LogTrace($"Nb files to backup : {_applicationStat.FilesToBackupCount}");

                fileListToBackup = RandomizeOrder(fileListToBackup);

                var filesInStorage = await _fileUploader.UploadIfNeeded(fileListToBackup, cancellationToken);
                await _fileUploader.CleanFilesAndDirectories(filesInStorage, fileListToBackup, cancellationToken);

                _logger.LogTrace("End ...");

                _logger.LogTrace(@$"Nb files already backuped : {_applicationStat.AlreadyBackupedCount}
Nb new files : {_applicationStat.NewFilesCount}
Nb updated files : {_applicationStat.UpdateFilesCount}
Nb deleted files : {_applicationStat.DeleteFilesCount}
Nb missing files : {_applicationStat.FilesToBackupCount - (_applicationStat.NewFilesCount + _applicationStat.NewFilesCount)}
Nb files in error : {_applicationStat.FilesInErrors.Count}");

                await _mailSender.SendRecapMailWithSendGrid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during the backup");
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        private List<FileItem> RandomizeOrder(List<FileItem> fileListToBackup)
        {
            return fileListToBackup.OrderBy(_ => Guid.NewGuid()).Select(x => x).ToList();
        }
    }
}
