using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, string containerName);
        Task<bool> DeleteImageAsync(string blobName, string containerName);
        Task<byte[]> DownloadImageAsync(string blobName, string containerName);
        string GetImageUrl(string blobName, string containerName);
    }
}
