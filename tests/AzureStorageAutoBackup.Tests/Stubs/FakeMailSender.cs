using AzureStorageAutoBackup.Mail;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Tests.Stubs
{
    public class FakeMailSender : IMailSender
    {
        public Task SendRecapMailWithSendGrid()
        {
            return Task.CompletedTask;
        }
    }
}
