using Application.Dtos;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace Presentation.Controllers;

[ApiController]
[AuthorizeWithRoles(Role.Buyer,Role.SystemAdministrator)]
[Route("api/payments")]
public class PaymentController(IPaymentService paymentService) : ControllerBase
{
    [HttpPost("buy-product")]
    public async Task<IActionResult> BuyProduct(PaymentRequestDto paymentRequestDto)
    {
        var paymentResult = await paymentService.ProcessPaymentAsync(paymentRequestDto);
        if (paymentResult)
        {
            return Ok();
        }

        return BadRequest();
    }
}