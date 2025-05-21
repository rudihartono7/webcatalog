using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IAIFWebCatalog.Services
{
    public class AzureStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"];
            _containerName = configuration["AzureStorage:ContainerName"];
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            // Create a unique file name to prevent overwriting
            string uniqueFileName = $"{Guid.NewGuid()}-{fileName}";
            
            // Get a reference to a container
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _containerName);
            
            // Create the container if it doesn't exist
            await containerClient.CreateIfNotExistsAsync();
            
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(uniqueFileName);
            
            // Upload the file
            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
            
            // Return the file name for storage in the database
            return uniqueFileName;
        }

        public async Task DeleteFileAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
                
            // Get a reference to a container
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _containerName);
            
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            
            // Delete the blob
            await blobClient.DeleteIfExistsAsync();
        }

        public string GetBlobUrl(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
                
            // Get a reference to a container
            BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _containerName);
            
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            
            // Return the blob URL
            return blobClient.Uri.ToString();
        }
    }
}