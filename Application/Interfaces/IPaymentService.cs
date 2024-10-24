using Application.Dtos;

namespace Application.Interfaces;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(PaymentRequestDto paymentRequestDto);
    Task<string> CreatePaymentMethodAsync(CreatePaymentMethodDto createPaymentMethodDto);
}