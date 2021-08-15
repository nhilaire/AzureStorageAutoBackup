using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Files
{
    public class FilesBuilder
    {
        private readonly AppConfiguration _appConfiguration;
        private readonly IFileReader _fileReader;

        public FilesBuilder(IOptions<AppConfiguration> appConfiguration, IFileReader fileReader)
        {
            _appConfiguration = appConfiguration.Value;
            _fileReader = fileReader;
        }

        public async Task<List<FileItem>> GetFileListToBackup()
        {
            var result = new List<FileItem>();
            var files = await _fileReader.ReadAllFiles(_appConfiguration.Paths);
            foreach (var currentFile in files)
            {
                if (!IsExcludedPath(currentFile.Path))
                {
                    result.Add(currentFile);
                }
            }

            var concurrentResult = new ConcurrentBag<FileItem>();
            Parallel.ForEach(result, file =>
            {
                concurrentResult.Add(FileItem.Create(file.Path, CalculateMD5(file.Path)));
            });

            return concurrentResult.ToList();
        }

        private bool IsExcludedPath(string file)
        {
            if (_appConfiguration.ExcludedPaths == null)
            {
                return false;
            }
            foreach (var excludedPath in _appConfiguration.ExcludedPaths)
            {
                if (Path.GetDirectoryName(file).Contains(excludedPath))
                {
                    return true;
                }
            }
            return false;
        }

        private string CalculateMD5(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = _fileReader.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

}
