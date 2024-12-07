using Microsoft.AspNetCore.Http;

namespace Application.Dtos;

public class CategoryDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IFormFile PhotoUrl { get; set; }
}
