using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/file/")]
public class FileStorageController(IFileStorageService fileStorageService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromQuery] string folder = "avatars")
    {
        try
        {
            var url = await fileStorageService.UploadPhotoAsync(file,folder);
            return Ok(new { Url = url });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading file: {ex.Message}");
        }
    }
}