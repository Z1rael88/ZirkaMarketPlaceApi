using Microsoft.AspNetCore.Http;

namespace Application.Dtos;

public class CreateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile PhotoUrl { get; set; }
    public int AvailableAmount  { get; set; }
}