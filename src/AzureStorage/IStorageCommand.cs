using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.AzureStorage
{
    public interface IStorageCommand
    {
        Task CreateDirectories(List<string> directories);
        Task UploadToStorage(FileItem file);
        Task DeleteFiles(List<string> filesToDelete);
        Task DeleteEmptyDirectoriesIfExist();
    }
}
