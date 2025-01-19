using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PurchaseRepository(IApplicationDbContext dbContext) : IPurchaseRepository
{
    public async Task<Purchase> CreatePurchaseAsync(Purchase purchase)
    {
        await dbContext.Purchases.AddAsync(purchase);
        return purchase;
    }

    public async Task<Purchase> GetPurchaseByIdAsync(Guid purchaseId)
    {
        return await dbContext.Purchases
            .FirstOrDefaultAsync(p => p.Id == purchaseId);
    }

    public async Task UpdatePurchaseStatusAsync(Guid purchaseId, PurchaseStatus status)
    {
        var purchase = await dbContext.Purchases.FindAsync(purchaseId);
        if (purchase != null)
        {
            purchase.Status = status;
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Purchase>> GetPurchasesByBuyerIdAsync(Guid buyerId)
    {
        return await dbContext.Purchases    
            .Include(p => p.Product)
            .Where(p => p.UserId == buyerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Purchase>> GetPurchasesBySellerIdAsync(Guid sellerId)
    {
        return await dbContext.Purchases
            .Include(p => p.Product)
            .Where(p => p.SellerId == sellerId)
            .ToListAsync();
    }
    
    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}