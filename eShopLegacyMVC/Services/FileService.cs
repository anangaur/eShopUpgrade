using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eShopLegacyMVC.Services
{
    public class FileService
    {
        private readonly FileServiceConfiguration _configuration;

        public FileService(FileServiceConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IEnumerable<string> ListFiles()
        {
            return Directory.GetFiles(_configuration.BasePath).Select(Path.GetFileName);
        }

        public byte[] DownloadFile(string filename)
        {
            var path = Path.Combine(_configuration.BasePath, filename);
            return File.ReadAllBytes(path);
        }

        public async Task UploadFileAsync(IFormFileCollection files)
        {
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file.FileName);
                var path = Path.Combine(_configuration.BasePath, filename);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
        }
    }

    public class FileServiceConfiguration
    {
        public string BasePath { get; set; }
        public string ServiceAccountId { get; set; }
        public string ServiceAccountDomain { get; set; }
        public string ServiceAccountPassword { get; set; }
    }
}
