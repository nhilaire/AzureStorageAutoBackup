using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AzureStorageAutoBackup.Tests.Stubs
{
    public class FakeConfiguration : IOptions<AppConfiguration>
    {
        public AppConfiguration Value => new AppConfiguration
        {
            Paths = new List<string>
            {
                "C:\\Source\\AzureStorageAutoBackup\\src",
                "C:\\Source\\AzureStorageAutoBackup\\tests",
                "C:\\Source\\AzureStorageAutoBackup",
            },
            ExcludedPaths = new List<string>
            {
                "C:\\Source\\AzureStorageAutoBackup\\src\\bin",
                "C:\\Source\\AzureStorageAutoBackup\\src\\obj",
                "C:\\Source\\AzureStorageAutoBackup\\tests\\AzureStorageAutoBackup.Tests\\bin",
                "C:\\Source\\AzureStorageAutoBackup\\tests\\AzureStorageAutoBackup.Tests\\obj",
                "C:\\Source\\AzureStorageAutoBackup\\.git",
            }
        };
    }
}
