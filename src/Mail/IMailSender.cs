using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Mail
{
    public interface IMailSender
    {
        Task SendRecapMailWithSendGrid();
    }
}