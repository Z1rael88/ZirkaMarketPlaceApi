using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController(IPaymentService paymentService) : ControllerBase
{
    [HttpPost("buy")]
    public async Task<IActionResult> BuyProduct(PaymentRequestDto paymentRequestDto)
    {
        var paymentResult = await paymentService.ProcessPaymentAsync(paymentRequestDto);
        if (paymentResult)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpPost("payment-method")]
    public async Task<IActionResult> CreatePaymentMethod(CreatePaymentMethodDto paymentRequestDto)
    {
        var paymentMethodId = await paymentService.CreatePaymentMethodAsync(paymentRequestDto);

        return Ok(paymentMethodId);
    }
}