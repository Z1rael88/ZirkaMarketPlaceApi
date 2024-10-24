using Application.Dtos;
using Domain.Filters;
using Domain.Models;

namespace Application.Interfaces;

public interface IProductService
{
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto productDto);
    Task<ProductResponseDto> UpdateProductAsync(ProductDto productDto,Guid productId);
    Task<ProductResponseDto> GetProductByIdAsync(Guid productId);
    Task<PaginatedResponse<ProductResponseDto>> GetAllPaginatedProductsAsync(int pageNumber, int pageSize,ProductFilter? filter = null);
    Task UpdateRatingAsync(Guid productId,int rating);
    Task DeleteProductAsync(Guid productId);
}