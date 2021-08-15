using System.Collections.Generic;

namespace AzureStorageAutoBackup
{
    public class AppConfiguration
    {
        public List<string> Paths { get; set; }
        public List<string> ExcludedPaths { get; set; }
        public string DestinationPathInAzure { get; set; }
        public string ConnectionString { get; set; }
        public string ShareReference { get; set; }
    }
}
