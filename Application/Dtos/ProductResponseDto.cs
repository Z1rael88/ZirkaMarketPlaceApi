namespace Application.Dtos;

public class ProductResponseDto 
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public int TotalAmountSold { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string PhotoUrl { get; set; }
    public int AvailableAmount  { get; set; }
    public Guid UserId { get; set; }
    public ICollection<PurchaseItemDto>? Purchases { get; set; }
}