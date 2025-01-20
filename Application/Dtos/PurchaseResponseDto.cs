using Domain.Models;

namespace Application.Dtos;

public class PurchaseResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } 
    public Guid SellerId { get; set; }
    public ProductResponseDto Product { get; set; }
    public int Quantity { get; set; } 
    public PurchaseStatus Status { get; set; }
}