using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace  Infrastructure.Repositories;
    
public class PurchaseRepository(IApplicationDbContext dbContext) : IPurchaseRepository
{
    public async Task<Purchase> CreatePurchaseAsync(Purchase purchase)
    {
        await dbContext.Purchases.AddAsync(purchase);
        return purchase;
    }

    public async Task<IEnumerable<Purchase>> GetPurchasesByUserIdAsync(Guid userId)
    {
        return await dbContext.Purchases
            .Include(p => p.Product)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}