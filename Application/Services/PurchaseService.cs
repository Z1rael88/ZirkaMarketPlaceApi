using Application.Interfaces;
using Domain.Models;
using Mapster;
using Application.Dtos;
using Infrastructure.Interfaces;

namespace Application.Services;

public class PurchaseService(IPurchaseRepository purchaseRepository, IProductRepository productRepository) : IPurchaseService
{
    public async Task<PurchaseDto> CreatePurchaseAsync(Guid buyerId, Guid productId, int quantity)
    {
        var product = await productRepository.GetProductByIdAsync(productId);
        if (product == null)
            throw new Exception("Product not found");
        
        var sellerId = product.UserId;
        if (sellerId == Guid.Empty)
            throw new Exception("Product has no associated seller"); 
        
        product.AvailableAmount -= quantity;
        if (product.AvailableAmount < quantity)
            throw new Exception("Insufficient product quantity available");

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            UserId = buyerId,
            SellerId = sellerId,
            ProductId = productId,
            Quantity = quantity
        };

        await purchaseRepository.CreatePurchaseAsync(purchase);
        await purchaseRepository.SaveChangesAsync();
       

        return purchase.Adapt<PurchaseDto>();
    }

    public async Task<IEnumerable<PurchaseDto>> GetPurchasesByBuyerIdAsync(Guid buyerId)
    {
        var purchases = await purchaseRepository.GetPurchasesByBuyerIdAsync(buyerId);
        return purchases.Adapt<IEnumerable<PurchaseDto>>();
    }
    public async Task<IEnumerable<PurchaseDto>> GetPurchasesBySellerIdAsync(Guid sellerId)
    {
        var purchases = await purchaseRepository.GetPurchasesBySellerIdAsync(sellerId);
        return purchases.Adapt<IEnumerable<PurchaseDto>>();
    }
}