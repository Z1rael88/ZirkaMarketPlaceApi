namespace Application.Dtos;

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }
    public List<ProductResponseDto>? Products { get; set; }
}
