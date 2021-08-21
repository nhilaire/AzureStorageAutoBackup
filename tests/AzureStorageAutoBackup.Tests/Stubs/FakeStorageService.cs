using AzureStorageAutoBackup.AzureStorage;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Tests.Stubs
{
    public class FakeStorageService : IStorageReader, IStorageCommand
    {
        public int Count { get; private set; } = 0;

        public Task<List<FileItem>> BrowseStorage()
        {
            return Task.FromResult(new List<FileItem>
            {
                new FileItem { Path = "C:\\Source\\AzureStorageAutoBackup\\README.md", Checksum = "61b979d5b9b2f8ee6dd5d81ed01ed39e"},
                new FileItem { Path = "C:\\Source\\AzureStorageAutoBackup\\src\\Files\\FilesBuilder.cs", Checksum = "56644ab2bbcf71d487b475f3c22632ad"},
                new FileItem { Path = "C:\\Source\\AzureStorageAutoBackup\\tests\\AzureStorageAutoBackup.Tests\\Testconfiguration.cs", Checksum = "ddddd"},
                new FileItem { Path = "C:\\Source\\AzureStorageAutoBackup\\src\\AzureStorageAutoBackup.sln", Checksum = "ffffff"},
                new FileItem { Path = "C:\\Source\\AzureStorageAutoBackup\\src\\NONEXISTINGFILE.sln", Checksum = "gggggggg"},
            });
        }

        public Task CreateDirectories(List<string> directories)
        {
            directories.Count.Should().Be(1);
            return Task.CompletedTask;
        }

        public Task DeleteEmptyDirectoriesIfExist()
        {
            return Task.CompletedTask;
        }

        public Task DeleteFiles(List<string> filesToDelete)
        {
            filesToDelete.Count.Should().Be(1);
            return Task.CompletedTask;
        }

        public Task UploadToStorage(FileItem file)
        {
            Count++;
            return Task.CompletedTask;
        }
    }
}
