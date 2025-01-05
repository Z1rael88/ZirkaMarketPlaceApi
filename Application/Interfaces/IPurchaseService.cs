using Application.Dtos;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IPurchaseService
    {
        Task<Purchase> CreatePurchaseAsync(Guid userId, Guid productId, int quantity);
        Task<IEnumerable<Purchase>> GetPurchasesByUserIdAsync(Guid userId);
    }
}