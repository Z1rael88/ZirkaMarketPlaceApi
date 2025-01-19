using Domain.Models;
namespace Application.Dtos;

public class PurchaseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } 
    public Guid SellerId { get; set; }
    public Guid ProductId { get; set; } 
    public int Quantity { get; set; } 
    public PurchaseStatus Status { get; set; }
}