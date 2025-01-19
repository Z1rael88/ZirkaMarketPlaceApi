using Application.Dtos;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;


namespace Presentation.Controllers;

[Route("api/purchase")]
[ApiController]
public class PurchaseController(IPurchaseService purchaseService) : ControllerBase
{
    [HttpGet("buyer/{userId}")]
    public async Task<IActionResult> GetPurchasesAsBuyer(Guid userId)
    {
        var purchases = await purchaseService.GetPurchasesByBuyerIdAsync(userId);
        return Ok(purchases);
    }
    
    [HttpGet("seller/{userId}")]
    public async Task<IActionResult> GetPurchasesAsSeller(Guid userId)
    {
        var purchases = await purchaseService.GetPurchasesBySellerIdAsync(userId);
        return Ok(purchases);
    }
    
    [HttpGet("seller/{userId}")]
    public async Task<IActionResult> GetPurchasesAsSeller(Guid userId)
    {
        var purchases = await purchaseService.GetPurchasesBySellerIdAsync(userId);
        return Ok(purchases);
    }
    
    [HttpPatch]
    public async Task<IActionResult> UpdateStatus(Guid purchaseId, PurchaseStatus status)
    {
        var updatedPurchase = await purchaseService.UpdatePurchaseStatusAsync(purchaseId, status);
        return Ok(updatedPurchase);
    }
}