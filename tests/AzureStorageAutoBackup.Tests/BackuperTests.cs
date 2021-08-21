using AzureStorageAutoBackup.AzureStorage;
using AzureStorageAutoBackup.Files;
using AzureStorageAutoBackup.Tests.Stubs;
using FluentAssertions;
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
            var fileReader = new FakeFileReader();
            var storageService = new FakeStorageService();
            var fileUploader = new FileUploader(storageService, storageService, filesState, new FakeLogger<FileUploader>(), new Md5(fileReader));
            var backuper = new Backuper(new FilesBuilder(appConfiguration, fileReader, new FakeLogger<FilesBuilder>()), filesState, fileUploader, new FakeLogger<Backuper>());
            await backuper.Run();
            storageService.Count.Should().Be(10);
            filesState.Count.Should().Be(2);
        }
    }
}
