using Application.Dtos;

namespace Application.Interfaces;

public interface IProductService
{
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto productDto);
    Task<ProductResponseDto> UpdateProductAsync(ProductDto productDto,Guid productId);
    Task<ProductResponseDto> GetProductByIdAsync(Guid productId);
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    Task DeleteProductAsync(Guid productId);
}