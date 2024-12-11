using Stripe.Climate;

namespace Application.Dtos;

public class BaseUserResponseDto : BaseUserDto
{
    public Guid Id { get; set; }
    public ICollection<Product> Products { get; set; }
}