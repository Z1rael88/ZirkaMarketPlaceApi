
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadPhotoAsync(IFormFile file, string folder);
}