using Domain.Filters;
using Domain.Models;

namespace Infrastructure.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<Product> GetProductByIdAsync(Guid productId);
    Task<PaginatedResponse<Product>> GetAllPaginatedProductsAsync(int pageNumber,int pageSize,ProductFilter? filter = null);
    Task<IEnumerable<Product>> GetBestSellersAsync();
    Task<IEnumerable<Product>> GetNewProductsAsync();
    Task DeleteProductAsync(Guid productId);
}