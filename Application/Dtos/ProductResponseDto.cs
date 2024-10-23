namespace Application.Dtos;

public class ProductResponseDto : ProductDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public int TotalAmountSold { get; set; }
}