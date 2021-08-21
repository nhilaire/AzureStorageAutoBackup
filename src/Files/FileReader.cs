using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Files
{
    public class FileReader : IFileReader
    {
        public async Task<List<FileItem>> ReadAllFiles(List<string> basePath)
        {
            var result = new List<FileItem>();

            foreach (var path in basePath)
            {
                await BrowseFiles(path, result);
            }
            return result.DistinctBy(x => x.Path).ToList();
        }

        private async Task BrowseFiles(string path, List<FileItem> result)
        {
            var files = new string[0];
            try
            {
                files = Directory.GetFiles(path);
            }
            catch (UnauthorizedAccessException)
            {
            }

            foreach (var file in files)
            {
                result.Add(new FileItem { Path = file });
            }

            var directories = new string[0];
            try
            {
                directories = Directory.GetDirectories(path);
            }
            catch (UnauthorizedAccessException)
            {
            }
            
            foreach (var directory in directories)
            {
                await BrowseFiles(directory, result);
            }
        }

        public Stream OpenRead(string filename)
        {
            return File.OpenRead(filename);
        }
    }
}
