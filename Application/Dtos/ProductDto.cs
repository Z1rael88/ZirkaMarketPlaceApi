using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Dtos;

public class ProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile PhotoUrl { get; set; }
    public int AvailableAmount  { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public ProductStatus Status { get; set; }
}