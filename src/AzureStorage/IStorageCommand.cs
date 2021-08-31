using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.AzureStorage
{
    public interface IStorageCommand
    {
        Task CreateDirectories(List<string> directories, CancellationTokenSource cancellationToken);
        Task UploadToStorage(FileItem file, CancellationTokenSource cancellationToken);
        Task DeleteFiles(List<string> filesToDelete, CancellationTokenSource cancellationToken);
        Task DeleteEmptyDirectoriesIfExist(CancellationTokenSource cancellationToken);
    }
}
