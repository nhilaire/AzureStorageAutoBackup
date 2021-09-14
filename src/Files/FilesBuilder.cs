using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageAutoBackup.Files
{
    public class FilesBuilder
    {
        private readonly AppConfiguration _appConfiguration;
        private readonly IFileReader _fileReader;
        private readonly ILogger<FilesBuilder> _logger;

        public FilesBuilder(IOptions<AppConfiguration> appConfiguration, IFileReader fileReader, ILogger<FilesBuilder> logger)
        {
            _appConfiguration = appConfiguration.Value;
            _fileReader = fileReader;
            _logger = logger;
        }

        public async Task<List<FileItem>> GetFileListToBackup()
        {
            var result = new List<FileItem>();
            _logger.LogTrace("Browsing files, please wait ...");
            var files = await _fileReader.ReadAllFiles(_appConfiguration.Paths);
            foreach (var currentFile in files)
            {
                if (!IsExcludedPath(currentFile.Path))
                {
                    result.Add(currentFile);
                }
            }

            return result;
        }

        private bool IsExcludedPath(string file)
        {
            if (_appConfiguration.ExcludedPaths == null)
            {
                return false;
            }
            foreach (var excludedPath in _appConfiguration.ExcludedPaths)
            {
                if (Path.GetDirectoryName(file).Contains(excludedPath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
