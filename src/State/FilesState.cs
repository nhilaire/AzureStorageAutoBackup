using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.State
{
    public class FilesState : IFilesState
    {
        private const string stateFileName = "files.state";
        public List<FileItem> CompletedFiles { get; set; }

        public async Task Load()
        {
            var filesStatPath = GetFilesStatPath();
            if (File.Exists(filesStatPath))
            {
                var json = await File.ReadAllTextAsync(filesStatPath);
                CompletedFiles = JsonConvert.DeserializeObject<List<FileItem>>(json);
            }
            else
            {
                CompletedFiles = new List<FileItem>();
            }
        }

        public async Task Save(FileItem file)
        {
            var existingFile = CompletedFiles.FirstOrDefault(x => string.Compare(x.Path, file.Path, true) == 0);
            if (existingFile == null)
            {
                CompletedFiles.Add(file);
            }
            else
            {
                existingFile.Checksum = file.Checksum;
            }
            var filesStatPath = GetFilesStatPath();
            var json = JsonConvert.SerializeObject(CompletedFiles);
            await File.WriteAllTextAsync(filesStatPath, json);
        }

        public async Task Delete(string filePath)
        {
            var existingFile = CompletedFiles.FirstOrDefault(x => string.Compare(x.Path, filePath, true) == 0);
            if (existingFile != null)
            {
                CompletedFiles.Remove(existingFile);
                var filesStatPath = GetFilesStatPath();
                var json = JsonConvert.SerializeObject(CompletedFiles);
                await File.WriteAllTextAsync(filesStatPath, json);
            }
        }

        private string GetFilesStatPath()
        {
            string basePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.Combine(Path.GetDirectoryName(basePath), stateFileName);
        }
    }
}
