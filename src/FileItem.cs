using Newtonsoft.Json;

namespace AzureStorageAutoBackup
{
    public class FileItem
    {
        public string Path { get; set; }
        public string Checksum { get; set; }
        [JsonIgnore]
        public FileState State { get; set; }

        public FileItem()
        {
        }

        private FileItem(string filePath, string checksum, FileState fileState)
        {
            Path = filePath;
            Checksum = checksum;
            State = fileState;
        }

        public static FileItem Create(string filePath)
        {
            return new FileItem(filePath, null, FileState.New);
        }

        public static FileItem Create(string filePath, string checksum)
        {
            return new FileItem(filePath, checksum, FileState.New);
        }

        public static FileItem Create(string filePath, string checksum, FileState fileState)
        {
            return new FileItem(filePath, checksum, fileState);
        }
    }
}
