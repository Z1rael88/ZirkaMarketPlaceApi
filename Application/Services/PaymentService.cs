using Application.Dtos;
using Application.Interfaces;
using Infrastructure.Interfaces;
using Stripe;

namespace Application.Services;

public class PaymentService(IProductRepository productRepository) : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(PaymentRequestDto paymentRequestDto)
    {
        decimal totalAmount = await CalculateTotalAmount(paymentRequestDto.PurchaseItemDtos);

        var paymentIntent = await CreatePaymentIntent(paymentRequestDto,totalAmount);

        if (paymentIntent.Status == "succeeded")
        {
            await HandleSuccessfulPayment(paymentRequestDto.PurchaseItemDtos);
            return true;
        }

        return false;
    }

    private async Task<PaymentIntent> CreatePaymentIntent(PaymentRequestDto paymentRequestDto,decimal amount)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = paymentRequestDto.Currency,
            Description = paymentRequestDto.Description,
            PaymentMethod = paymentRequestDto.PaymentMethodId,
            Confirm = true,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never",
            }
           
        };
        var service = new PaymentIntentService();
        return await service.CreateAsync(options);
    }

    private async Task HandleSuccessfulPayment(List<PurchaseItemDto> purchaseItems)
    {
        var productIds = purchaseItems.Select(p => p.ProductId).ToList();
        var products = await productRepository.GetProductsByIdsAsync(productIds);

        foreach (var purchaseItem in purchaseItems)
        {
            var product = products.FirstOrDefault(p => p.Id == purchaseItem.ProductId);
            if (product != null)
            {
                product.AvailableAmount -= purchaseItem.Quantity;
                product.TotalAmountSold += purchaseItem.Quantity;
               await productRepository.SaveChangesAsync();
            }
        }
    }

    private async Task<decimal> CalculateTotalAmount(List<PurchaseItemDto> purchaseItems)
    {
        decimal total = 0;
        var productIds = purchaseItems.Select(p => p.ProductId).ToList();
        var products = await productRepository.GetProductsByIdsAsync(productIds);

        foreach (var purchaseItem in purchaseItems)
        {
            var product = products.FirstOrDefault(p => p.Id == purchaseItem.ProductId);
            if (product != null && product.AvailableAmount >= purchaseItem.Quantity)
            {
                total += product.Price * purchaseItem.Quantity;
            }
        }

        return total;
    }
}