using Application.Dtos;
using Domain.Enums;
using Domain.Models;


namespace Application.Interfaces;

public interface IPurchaseService
{
    Task<PurchaseResponseDto> CreatePurchaseAsync(Guid buyerId, Guid productId, int quantity);
    Task<PurchaseResponseDto> UpdatePurchaseStatusAsync(Guid purchaseId, PurchaseStatus status);
    Task<IEnumerable<PurchaseResponseDto>> GetPurchasesByBuyerIdAsync(Guid buyerId);
    Task<IEnumerable<PurchaseResponseDto>> GetPurchasesBySellerIdAsync(Guid sellerId);
}