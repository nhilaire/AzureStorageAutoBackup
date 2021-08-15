using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.State
{
    public interface IFilesState
    {
        List<FileItem> CompletedFiles { get; }

        Task Save(FileItem file);
        Task Load();
        Task Delete(string filePath);
    }
}
