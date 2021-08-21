using AzureStorageAutoBackup.State;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Tests.Stubs
{
    public class FakeFilesState : IFilesState
    {
        public int Count { get; private set; }
        private List<FileItem> _completedFiles;
        public List<FileItem> CompletedFiles => _completedFiles;

        public FakeFilesState()
        {
            _completedFiles = new List<FileItem>
            {
                new FileItem { Path = "C:\\Source\\AzureStorageAutoBackup\\src\\AppConfiguration.cs", Checksum = "c393c7b0e9b44e23d8f57c31a7e97222"},
                new FileItem{ Path = "C:\\Source\\AzureStorageAutoBackup\\src\\Program.cs", Checksum = "aaaa"}
            };
        }

        public Task Delete(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public Task Load()
        {
            return Task.CompletedTask;
        }

        public Task Save(FileItem file)
        {
            Count++;
            return Task.CompletedTask;
        }
    }
}
