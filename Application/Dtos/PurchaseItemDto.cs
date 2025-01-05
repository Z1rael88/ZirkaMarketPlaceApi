namespace Application.Dtos;

public class PurchaseItemDto
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}