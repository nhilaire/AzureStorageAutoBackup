using System.Collections.Generic;
using System.Linq;

namespace AzureStorageAutoBackup
{
    public static class FileItemComparer
    {
        public static bool ExistsIn(this FileItem file, List<FileItem> alreadyBackupedFiles)
        {
            var currentFile = alreadyBackupedFiles.FirstOrDefault(x => x.Path == file.Path);
            return currentFile != null && currentFile.Checksum == file.Checksum;
        }
    }
}
