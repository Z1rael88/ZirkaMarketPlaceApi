using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(IApplicationDbContext dbContext) : IProductRepository
{
    public async Task<Product> CreateProductAsync(Product product)
    {
        var createdProduct = await dbContext.Products.AddAsync(product);
        if (createdProduct == null)
            throw new ArgumentException("Product creation failed");
        await dbContext.SaveChangesAsync();
        return createdProduct.Entity;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        var productToUpdate = await GetProductByIdAsync(product.Id);
        productToUpdate.Description = product.Description;
        productToUpdate.Name = product.Name;
        productToUpdate.Rating = product.Rating;
        await dbContext.SaveChangesAsync();
        return productToUpdate;
    }

    public async Task<Product> GetProductByIdAsync(Guid productId)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
            throw new ArgumentException($"Project with id : {productId} not found");
        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await dbContext.Products.ToListAsync();
    }

    public async Task DeleteProductAsync(Guid productId)
    {
        await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
    }
}