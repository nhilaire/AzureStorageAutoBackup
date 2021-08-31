using System.Collections.Generic;

namespace AzureStorageAutoBackup.State
{
    public class ApplicationStat
    {
        public ApplicationStat()
        {
            FilesInErrors = new List<string>();
        }

        public int FilesToBackupCount { get; internal set; }
        public int MissingDirectoriesCount { get; internal set; }
        public int AlreadyBackupedCount { get; internal set; }
        public int NewFilesCount { get; internal set; }
        public int UpdateFilesCount { get; internal set; }
        public int DeleteFilesCount { get; internal set; }
        public List<string> FilesInErrors { get; internal set; }
    }
}
