using Domain.Enums;

namespace Domain.Models;
public enum PurchaseStatus
{
    Bought = 0,
    Sent = 1,
    Delivered = 2
}
public class Purchase : BaseEntity
{
    public Guid SellerId { get; set; }
    public User Seller { get; set; }
    public Guid UserId { get; set; }
    public User Buyer { get; set; } 
    public Guid ProductId { get; set; }
    public Product Product { get; set; } 
    public int Quantity { get; set; }
    public PurchaseStatus Status { get; set; }
}