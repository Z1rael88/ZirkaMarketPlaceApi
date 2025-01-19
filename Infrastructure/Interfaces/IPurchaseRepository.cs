using Domain.Models;

namespace Infrastructure.Interfaces;

public interface IPurchaseRepository
{
    Task<Purchase> CreatePurchaseAsync(Purchase purchase);
    Task<IEnumerable<Purchase>> GetPurchasesByBuyerIdAsync(Guid buyerId);
    Task<IEnumerable<Purchase>> GetPurchasesBySellerIdAsync(Guid sellerId);
    Task SaveChangesAsync();
}