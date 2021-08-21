using System;
using System.Security.Cryptography;

namespace AzureStorageAutoBackup.Files
{
    public class Md5
    {
        private readonly IFileReader _fileReader;

        public Md5(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public string CalculateMD5(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = _fileReader.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
