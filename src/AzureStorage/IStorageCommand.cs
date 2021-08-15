using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.AzureStorage
{
    public interface IStorageCommand
    {
        Task CreateDirectories(List<string> directories);
        Task UploadToStorage(List<FileItem> files);
        Task DeleteFiles(List<string> filesToDelete);
        Task DeleteEmptyDirectoriesIfExist();
    }
}
