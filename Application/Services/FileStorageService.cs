using Application.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public FileStorageService(IOptions<GoogleStorageOptions> options)
        {
            _bucketName = options.Value.BucketName;
            var credential = GoogleCredential.FromFile(options.Value.CredentialsPath);
            _storageClient = StorageClient.Create(credential);
        }

        public async Task<string> UploadPhotoAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty", nameof(file));

            var objectName = $"{folder}/{Guid.NewGuid()}_{file.FileName}";

            await using var stream = file.OpenReadStream();
            await _storageClient.UploadObjectAsync(_bucketName, objectName, file.ContentType, stream);
            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }
    }
}