using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Files
{
    public interface IFileReader
    {
        Stream OpenRead(string filename);
        Task<List<FileItem>> ReadAllFiles(List<string> basePath);
    }

}
