using Application.Dtos;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;


namespace Presentation.Controllers;

[Route("api/purchase")]
[ApiController]
public class PurchaseController(IPurchaseService purchaseService) : ControllerBase
{
    [AuthorizeWithRoles(Role.SystemAdministrator)]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetAllPurchases(Guid userId)
    {
        var purchases = await purchaseService.GetPurchasesByUserIdAsync(userId);
        return Ok(purchases);
    }
}