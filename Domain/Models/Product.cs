namespace Domain.Models;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public List<int>? Ratings { get; set; } = [];
    public decimal Price { get; set; }
    public string PhotoUrl { get; set; }
    public int AvailableAmount { get; set; }
    public int TotalAmountSold { get; set; }
    public Category Category { get; set; }
    public Guid CategoryId { get; set; }
    public User User { get; set; }
    public Guid UserId { get; set; }
    public Purchase Purchase { get; set; }  

}