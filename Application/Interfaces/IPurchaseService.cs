using Application.Dtos;


namespace Application.Interfaces;

public interface IPurchaseService
{
    Task<PurchaseDto> CreatePurchaseAsync(Guid userId, Guid productId, int quantity);
    Task<IEnumerable<PurchaseDto>> GetPurchasesByUserIdAsync(Guid userId);
}