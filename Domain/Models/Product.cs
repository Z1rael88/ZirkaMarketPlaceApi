namespace Domain.Models;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Rating { get; set; }
    public string PhotoUrl { get; set; }
    public int AvailableAmount { get; set; }
    public int TotalAmountSold { get; set; }
}