using Domain.Models;

namespace Infrastructure.Interfaces;

public interface IPurchaseRepository
{
    Task<Purchase> CreatePurchaseAsync(Purchase purchase);
    Task<IEnumerable<Purchase>> GetPurchasesByUserIdAsync(Guid userId);
    Task SaveChangesAsync();
}