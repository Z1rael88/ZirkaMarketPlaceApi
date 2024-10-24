namespace Application.Dtos;

public class PaymentRequestDto
{
    public List<PurchaseItemDto> PurchaseItemDtos { get; set; }
    public string Currency { get; set; }
    public string Description { get; set; }
    public string PaymentMethodId { get; set; }
}