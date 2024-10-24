using Domain.Filters;
using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(IApplicationDbContext dbContext) : IProductRepository
{
    public async Task<Product> CreateProductAsync(Product product)
    {
        var createdProduct = await dbContext.Products.AddAsync(product);
        createdProduct.Entity.Rating = 0;
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

    public async Task<PaginatedResponse<Product>> GetAllPaginatedProductsAsync(int pageNumber, int pageSize,
        ProductFilter? filter = null)
    {
        var query = dbContext.Products.AsQueryable();
        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(p => p.Name.Contains(filter.Name));
            }

            if (filter.Rating.HasValue)
                query = query.Where(p => p.Rating == filter.Rating);
            if (filter.Price.HasValue)
                query = query.Where(p => p.Price <= filter.Price);
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedResponse<Product>()
        {
            TotalCount = totalCount,
            Items = items,
            PageSize = pageSize,
            PageNumber = pageNumber
        };
    }

    public async Task DeleteProductAsync(Guid productId)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product != null) dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<Guid> productIds)
    {
        return await dbContext.Products
            .Where(p => productIds.Contains(p.Id))  
            .ToListAsync();                       
    }

}