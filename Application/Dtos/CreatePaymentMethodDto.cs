namespace Application.Dtos;

public class CreatePaymentMethodDto
{
    public string CardNumber { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public string Cvc { get; set; }
}