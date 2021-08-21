using Newtonsoft.Json;

namespace AzureStorageAutoBackup
{
    public class FileItem
    {
        public string Path { get; set; }
        public string Checksum { get; set; }
        [JsonIgnore]
        public FileState State { get; set; }
    }
}
