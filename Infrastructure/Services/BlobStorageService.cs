using Application.Services.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobStorageService> _logger;
        private const long MaxFileSize = 2097152; // 2 MB
        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            var connectionString = configuration.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _logger = logger;
        }
        public async Task<bool> DeleteImageAsync(string blobName, string containerName)
        {
            if (string.IsNullOrEmpty(blobName))
                return false;

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();

            _logger.LogInformation($"Successfully deleted image from Blob Storage: {blobName}");
            return true;
        }

        public async Task<byte[]> DownloadImageAsync(string blobName, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            //Download to memory
            using (var memoryStream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public string GetImageUrl(string blobName, string containerName)
        {
            if (string.IsNullOrEmpty(blobName))
                return string.Empty;

            //Build the URL
            var containerCLient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerCLient.GetBlobClient(blobName);

            return blobClient.Uri.ToString();
        }
        

        public async Task<string> UploadImageAsync(IFormFile file, string containerName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty", nameof(file));
            }
            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException("File size exceeds the maximum limit of 2 MB", nameof(file));
            }

            //Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Unsupported file type. Only JPEG, PNG, and GIF are allowed.", nameof(file));
            }

            //Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var blobName = $"{Guid.NewGuid()}{extension}";

            //Gets the "folder" (container) where you want to store the file
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            //Creates the container if it doesn't exist yet
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob); //PublicAccessType.Blob means files can be accessed via URL

            //Upload the file
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            _logger.LogInformation($"Successfully uploaded image to Blob Storage: {blobName}");
            return blobName;
        }
    }
}

