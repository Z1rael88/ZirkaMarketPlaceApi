using Microsoft.AspNetCore.Http;

namespace Application.Helpers;

public static class ConverterFromIFormFileToString
{
    public static async Task<string> ConvertIFormFileToBase64Async(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        return Convert.ToBase64String(fileBytes);
    }

}

