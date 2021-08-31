using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.AzureStorage
{
    public interface IStorageReader
    {
        Task<List<FileItem>> BrowseStorage(CancellationTokenSource cancellationToken);
    }
}
