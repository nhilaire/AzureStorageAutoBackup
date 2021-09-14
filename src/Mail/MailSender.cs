using AzureStorageAutoBackup.State;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Mail
{
    public class MailSender : IMailSender
    {
        private readonly AppConfiguration _appConfiguration;
        private readonly ApplicationStat _applicationStat;
        private readonly ILogger<MailSender> _logger;

        public MailSender(IOptions<AppConfiguration> options, ApplicationStat applicationStat, ILogger<MailSender> logger)
        {
            _appConfiguration = options.Value;
            _applicationStat = applicationStat;
            _logger = logger;
        }

        public async Task SendRecapMailWithSendGrid()
        {
            _logger.LogDebug("Sending mail ...");
            var apiKey = _appConfiguration.SendGridApiKey;
            var client = new SendGridClient(apiKey);

            foreach (var dest in _appConfiguration.MailsTo)
            {
                var from = new EmailAddress(dest, "Backuper");
                var subject = "Backup ended";
                var to = new EmailAddress(dest, dest);
                var plainTextContent = string.Empty;
                var htmlContent = @$"Backup ended with<br/>
Nb files already backuped : {_applicationStat.AlreadyBackupedCount}<br/>
Nb new files : { _applicationStat.NewFilesCount}<br/>
Nb updated files: { _applicationStat.UpdateFilesCount}<br/>
Nb deleted files: { _applicationStat.DeleteFilesCount}<br/>
Nb missing files : {_applicationStat.FilesToBackupCount - (_applicationStat.NewFilesCount + _applicationStat.UpdateFilesCount)}<br/>
Nb files in error : {_applicationStat.FilesInErrors.Count}<br/><br/>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                await client.SendEmailAsync(msg);
            }
        }
    }
}
