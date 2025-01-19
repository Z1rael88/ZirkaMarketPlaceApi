using Application.Dtos;
using Domain.Enums;
using Domain.Models;


namespace Application.Interfaces;

public interface IPurchaseService
{
    Task<PurchaseDto> CreatePurchaseAsync(Guid buyerId, Guid productId, int quantity);
    Task<PurchaseDto> UpdatePurchaseStatusAsync(Guid purchaseId, PurchaseStatus status);
    Task<IEnumerable<PurchaseDto>> GetPurchasesByBuyerIdAsync(Guid buyerId);
    Task<IEnumerable<PurchaseDto>> GetPurchasesBySellerIdAsync(Guid sellerId);

}