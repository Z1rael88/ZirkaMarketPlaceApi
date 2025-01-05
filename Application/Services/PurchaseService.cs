using Application.Interfaces;
using Domain.Models;
using Infrastructure.Interfaces;
 
namespace Application.Services
{
    public class PurchaseService(IPurchaseRepository purchaseRepository, IProductRepository  productRepository) : IPurchaseService
    {
 
        public async Task<Purchase> CreatePurchaseAsync(Guid userId, Guid productId, int quantity)
        {
            var product = await productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");
 
            var purchase = new Purchase
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
           
            };
 
            await purchaseRepository.CreatePurchaseAsync(purchase);
            await purchaseRepository.SaveChangesAsync();
 
            return purchase;
        }
 
        public async Task<IEnumerable<Purchase>> GetPurchasesByUserIdAsync(Guid userId)
        {
            return await purchaseRepository.GetPurchasesByUserIdAsync(userId);
        }
    }
}