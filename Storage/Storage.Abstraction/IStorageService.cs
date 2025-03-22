using Microsoft.AspNetCore.Http;

namespace Storage.Abstraction
{
    public interface IStorageService
    {
        Task<StorageObject> UploadFileAsync(IFormFile file, string bucketName, string? prefix = null);
        Task<StorageObject> GetFileAsync(string fileKey, string bucketName);
        Task DeleteFileAsync(string fileKey, string bucketName);
    }
}
