using AzureStorageAutoBackup.Files;
using AzureStorageAutoBackup.Tests.Stubs;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Tests
{
    [TestClass]
    public class FilesBuilderTests
    {
        [TestMethod]
        public async Task GetFileList_WithCorrectExcludedPath_AndChecksum()
        {
            var fileBuilder = new FilesBuilder(new FakeConfiguration(), new FakeFileReader(), new FakeLogger<FilesBuilder>());
            var files = await fileBuilder.GetFileListToBackup();
            files = files.OrderBy(x => x.Path).ToList();

            files.Count.Should().Be(13);
            files[0].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\.gitignore");
            files[1].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\.vscode\\launch.json");
            files[2].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\.vscode\\tasks.json");
            files[3].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\LICENSE");
            files[4].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\README.md");
            files[5].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\src\\AppConfiguration.cs");
            files[6].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\src\\AzureStorageAutoBackup.csproj");
            files[7].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\src\\AzureStorageAutoBackup.sln");
            files[8].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\src\\Files\\FilesBuilder.cs");
            files[9].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\src\\Program.cs");
            files[10].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\tests\\AzureStorageAutoBackup.Tests\\AzureStorageAutoBackup.Tests.csproj");
            files[11].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\tests\\AzureStorageAutoBackup.Tests\\FilesBuilderTests.cs");
            files[12].Path.Should().Be("C:\\Source\\AzureStorageAutoBackup\\tests\\AzureStorageAutoBackup.Tests\\Testconfiguration.cs");
        }
    }
}
