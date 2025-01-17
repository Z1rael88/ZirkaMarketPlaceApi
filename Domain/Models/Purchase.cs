using Domain.Enums;

namespace Domain.Models;

public class Purchase : BaseEntity
{
    public Guid SellerId { get; set; }
    public User Seller { get; set; }
    public Guid UserId { get; set; }
    public User Buyer { get; set; } 
    public Guid ProductId { get; set; } 
    public Product Product { get; set; } 
    public int Quantity { get; set; }
}