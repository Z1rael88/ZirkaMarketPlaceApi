using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(IApplicationDbContext dbContext) : IProductRepository
{
    public async Task<Product> CreateProductAsync(Product product)
    {
        var createdProduct = await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();
        return createdProduct.Entity;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        var productToUpdate = await GetProductByIdAsync(product.Id);
        dbContext.Entry(productToUpdate).CurrentValues.SetValues(product);
        dbContext.Entry(productToUpdate).Property(nameof(productToUpdate.TotalAmountSold)).IsModified = false;
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
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product != null) dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
    }
}