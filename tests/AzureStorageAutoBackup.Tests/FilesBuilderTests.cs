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
            var fileBuilder = new FilesBuilder(new FakeConfiguration(), new FakeFileReader());
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

            files[0].Checksum.Should().Be("633eff90fbc773ec365e9b399f7190a5");
            files[1].Checksum.Should().Be("8ee62746b2984e8a8dafad98847e1a5b");
            files[2].Checksum.Should().Be("3e569da31d20fa7b75cb82127199ade3");
            files[3].Checksum.Should().Be("3cbb9d66b3868c57d9a4f15b49cdb401");
            files[4].Checksum.Should().Be("61b979d5b9b2f8ee6dd5d81ed01ed39e");
            files[5].Checksum.Should().Be("c393c7b0e9b44e23d8f57c31a7e97222");
            files[6].Checksum.Should().Be("fbd506e270f54049ab2663756fe311c4");
            files[7].Checksum.Should().Be("6654c5ca11f8d85bed26e20a8e314796");
            files[8].Checksum.Should().Be("56644ab2bbcf71d487b475f3c22632ad");
            files[9].Checksum.Should().Be("fa11e97f89c342d82eda9bf0763101a8");
            files[10].Checksum.Should().Be("71aa4fc274e264c62aa85d73063de40c");
            files[11].Checksum.Should().Be("b5f7bb0d820cdb6f4cce2f825dceddb1");
            files[12].Checksum.Should().Be("4c8425034170289848c2fbbc88dbb04e");
        }
    }
}
