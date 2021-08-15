﻿using MoreLinq;
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
            foreach (var file in Directory.GetFiles(path))
            {
                result.Add(FileItem.Create(file));
            }

            foreach (var directory in Directory.GetDirectories(path))
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