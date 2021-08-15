using AzureStorageAutoBackup.AzureStorage;
using AzureStorageAutoBackup.Files;
using AzureStorageAutoBackup.Tests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Tests
{
    [TestClass]
    public class BackuperTests
    {
        [TestMethod]
        public async Task Backuper_Run()
        {
            var appConfiguration = new FakeConfiguration();
            var filesState = new FakeFilesState();
            var storageService = new FakeStorageService();
            var fileUploader = new FileUploader(storageService, storageService, filesState, new FakeLogger<FileUploader>());
            var backuper = new Backuper(new FilesBuilder(appConfiguration, new FakeFileReader()), filesState, fileUploader, new FakeLogger<Backuper>());
            await backuper.Run();
        }
    }
}
