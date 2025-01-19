using Application.Dtos;


namespace Application.Interfaces;

public interface IPurchaseService
{
    Task<PurchaseDto> CreatePurchaseAsync(Guid buyerId, Guid productId, int quantity);
    Task<IEnumerable<PurchaseDto>> GetPurchasesByBuyerIdAsync(Guid buyerId);
    Task<IEnumerable<PurchaseDto>> GetPurchasesBySellerIdAsync(Guid sellerId);
}