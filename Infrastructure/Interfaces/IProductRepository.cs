using Domain.Models;

namespace Infrastructure.Interfaces;

public interface IProductRepository
{
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<Product> GetProductByIdAsync(Guid productId);
    Task<IEnumerable<Product>> GetProductsAsync();
    Task DeleteProductAsync(Guid productId);
}