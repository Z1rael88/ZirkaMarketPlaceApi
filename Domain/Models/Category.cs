namespace Domain.Models;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }
}
