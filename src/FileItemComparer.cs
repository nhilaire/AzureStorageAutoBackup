using System.Collections.Generic;
using System.Linq;

namespace AzureStorageAutoBackup
{
    public static class FileItemComparer
    {
        public static List<FileItem> GetFilesIntersection(List<FileItem> list1, List<FileItem> list2)
        {
            var effectivesFiles = new List<FileItem>();
            foreach (var file in list1)
            {
                var currentFile = list2.FirstOrDefault(x => x.Path == file.Path);
                if (currentFile == null)
                {
                    effectivesFiles.Add(FileItem.Create(file.Path, file.Checksum, FileState.New));
                }
                else if (currentFile.Checksum != file.Checksum)
                {
                    effectivesFiles.Add(FileItem.Create(file.Path, file.Checksum, FileState.Updated));
                }
            }
            return effectivesFiles;
        }
    }
}
