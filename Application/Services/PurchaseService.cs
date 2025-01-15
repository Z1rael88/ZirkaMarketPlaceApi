using Application.Interfaces;
using Domain.Models;
using Mapster;
using Application.Dtos;
using Infrastructure.Interfaces;

namespace Application.Services;

public class PurchaseService(IPurchaseRepository purchaseRepository, IProductRepository productRepository) : IPurchaseService
{
    public async Task<PurchaseDto> CreatePurchaseAsync(Guid userId, Guid productId, int quantity)
    {
        var product = await productRepository.GetProductByIdAsync(productId);
        if (product == null)
            throw new Exception("Product not found");

        product.AvailableAmount -= quantity;

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = productId,
            Quantity = quantity
        };

        await purchaseRepository.CreatePurchaseAsync(purchase);
        await purchaseRepository.SaveChangesAsync();
       

        return purchase.Adapt<PurchaseDto>();
    }

    public async Task<IEnumerable<PurchaseDto>> GetPurchasesByUserIdAsync(Guid userId)
    {
        var purchases = await purchaseRepository.GetPurchasesByUserIdAsync(userId);
        return purchases.Adapt<IEnumerable<PurchaseDto>>();
    }
}