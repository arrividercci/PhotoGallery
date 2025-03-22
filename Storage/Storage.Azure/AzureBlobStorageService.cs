using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Storage.Abstraction;

namespace Storage.Azure
{
    public class AzureBlobStorageService : IStorageService
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly ILogger<AzureBlobStorageService> logger;

        public AzureBlobStorageService(BlobServiceClient blobServiceClient, ILogger<AzureBlobStorageService> logger)
        {
            this.blobServiceClient = blobServiceClient;
            this.logger = logger;
        }

        public async Task<StorageObject> GetFileAsync(string fileKey, string bucketName)
        {
            logger.LogInformation("Attempting to retrieve file with key={FileKey} from container={ContainerName}", fileKey, bucketName);
            BlobContainerClient? blobContainer = blobServiceClient.GetBlobContainerClient(bucketName);
            BlobClient? blob = blobContainer.GetBlobClient(fileKey);
            BlobProperties properties = await blob.GetPropertiesAsync();
            var sObject = new StorageObject
            {
                ContentType = properties.ContentType,
                Url = blob.Uri.AbsoluteUri,
            };
            return sObject;
        }

        public async Task<StorageObject> UploadFileAsync(IFormFile file, string bucketName, string? prefix = null)
        {
            logger.LogInformation("Start saving file with name={FileName} and content-type={ContentType} to container={ContainerName}",
                                  file.FileName,
                                  file.ContentType,
                                  bucketName);

            BlobContainerClient? blobContainer = blobServiceClient.GetBlobContainerClient(bucketName);
            BlobClient? blob = blobContainer.GetBlobClient(file.FileName);
            if (!blob.Exists())
            {
                logger.LogInformation("Saving file={FileName} to blob storage", file.FileName);
                await blob.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });
            }

            BlobProperties properties = await blob.GetPropertiesAsync();
            var sObject = new StorageObject
            {
                ContentType = properties.ContentType,
                Url = blob.Uri.AbsoluteUri,
            };
            return sObject;
        }

        public async Task DeleteFileAsync(string fileKey, string bucketName)
        {
            logger.LogInformation("Deleting file with key={FileKey} from container={ContainerName}", fileKey, bucketName);
            try
            {
                BlobContainerClient? blobContainer = blobServiceClient.GetBlobContainerClient(bucketName);
                BlobClient? blob = blobContainer.GetBlobClient(fileKey);
                await blob.DeleteIfExistsAsync();
                logger.LogInformation("File with key={FileKey} successfully deleted from container={ContainerName}", fileKey, bucketName);
            }
            catch (RequestFailedException ex)
            {
                logger.LogError(ex, "Error occurred while deleting file with key={FileKey} from container={ContainerName}", fileKey, bucketName);
                throw new Exception($"Error deleting file with key {fileKey} from bucket {bucketName}: {ex.Message}", ex);
            }
        }
    }
}
