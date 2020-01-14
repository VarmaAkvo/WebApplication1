using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Utilities
{
    public class FileUpload
    {
        private readonly IConfiguration config;
        private readonly string wwwrootPath;
        public FileUpload(IConfiguration config, IWebHostEnvironment env)
        {
            this.config = config;
            wwwrootPath = env.WebRootPath;
        }

        public class UploadResult
        {
            public bool Successed { get; set; }
            public string ErrorMessage { get; set; }
            public string Path { get; set; }
        }

        public async Task<UploadResult> UploadGravatarAsync(IFormFile file)
        {
            return await UploadAsync(file, "GravatarStorePath");
        }

        public async Task<UploadResult> UploadTrixAttachmentAsync(IFormFile file)
        {
            return await UploadAsync(file, "TrixAttachmentStorePath");
        }

        public async Task<UploadResult> UploadAsync(IFormFile file, string storeKey)
        {
            var result = new UploadResult();
            string untrustedFileName = Path.GetFileName(file.FileName);
            long fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            if (file.Length > fileSizeLimit)
            {
                result.Successed = false;
                result.ErrorMessage = $"The file's size is bigger than {fileSizeLimit}B.";
            }
            else
            {
                var filePath = Path.Combine(wwwrootPath,
                    config.GetValue<string>(storeKey),
                    DateTime.UtcNow.ToFileTimeUtc().ToString() + untrustedFileName);
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                result.Successed = true;
                result.Path = filePath;
            }
            return result;
        }

        public string GetGravatarUrl(string path)
        {
            return GetImageUrl(path, "GravatarStorePath");
        }

        public string GetForumIconUrl(string path)
        {
            return GetImageUrl(path, "ForumIconStorePath");
        }

        public string GetTrixAttachmentUrl(string path)
        {
            return GetImageUrl(path, "TrixAttachmentStorePath");
        }

        public string GetImageUrl(string path, string storeKey)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            string fileName = Path.GetFileName(path);
            return $"/{config.GetValue<string>(storeKey)}/{fileName}";
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteFileByUrl(string url)
        {
            string relativePath = url.Substring(1).Replace('/', '\\');
            string filePath = Path.Combine(wwwrootPath, relativePath);
            DeleteFile(filePath);
        }

        public void DeleteFiles(string[] paths)
        {
            foreach (var path in paths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    DeleteFile(path);
                }
            }
        }
    }
}
