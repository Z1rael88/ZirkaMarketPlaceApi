using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class User :IdentityUser<Guid>, IBaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public string? GoogleId { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<Purchase> PurchasesAsBuyer { get; set; } 
    public ICollection<Purchase> PurchasesAsSeller { get; set; }
}