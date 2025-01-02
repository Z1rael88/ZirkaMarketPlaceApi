using Domain.Filters;
using Domain.Models;
using Elastic.Clients.Elasticsearch;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories;

public class ProductRepository(ElasticsearchClient client,IOptions<ElasricsearchOptions> elasricsearchOptions,IApplicationDbContext dbContext) : IProductRepository
{
    public async Task<Product> CreateProductAsync(Product product)
    {
        var createdProduct = await dbContext.Products.AddAsync(product);
        createdProduct.Entity.Rating = 0;
        await dbContext.SaveChangesAsync();
        await client.IndexAsync(product);
        return createdProduct.Entity;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        var productToUpdate = await GetProductByIdAsync(product.Id);
        dbContext.Entry(productToUpdate).CurrentValues.SetValues(product);
        dbContext.Entry(productToUpdate).Property(nameof(productToUpdate.TotalAmountSold)).IsModified = false;
        await dbContext.SaveChangesAsync();
        await client.IndexAsync(product);
        return productToUpdate;
    }

    public async Task<Product> GetProductByIdAsync(Guid productId)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
            throw new ArgumentException($"Project with id : {productId} not found");
        return product;
    }

    public async Task<PaginatedResponse<Product>> GetAllPaginatedProductsAsync(
        int pageNumber,
        int pageSize,
        ProductFilter? filter = null)
    {
        await CheckIfIndexExists();

        var searchDescriptor = new SearchRequestDescriptor<Product>()
            .Index(elasricsearchOptions.Value.DefaultIndex)
            .From((pageNumber - 1) * pageSize)
            .Size(pageSize);

        if (filter is not null)
        {
            searchDescriptor.Query(q => q.Bool(b =>
            {
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    b.Must(m => m.Match(mq => mq.Field(f => f.Name).Query(filter.Name)));
                }

                if (filter.Rating.HasValue)
                {
                    b.Must(m => m.Term(t => t.Field(f => f.Rating).Value(filter.Rating.Value)));
                }
            }));
        }

        var response = await client.SearchAsync(searchDescriptor);
        if (!response.IsValidResponse)
        {
            throw new ArgumentException("CHUJLANSUKA");
        }


        return new PaginatedResponse<Product>
        {
            TotalCount = (int)response.Total,
            Items = response.Documents.ToList(),
            PageSize = pageSize,
            PageNumber = pageNumber
        };
    }


    public async Task DeleteProductAsync(Guid productId)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product != null) dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        await client.IndexAsync(product);
    }

    public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<Guid> productIds)
    {
        return await dbContext.Products
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetBestSellersAsync()
    {
        await CheckIfIndexExists();
        var sortedProducts = await client.SearchAsync<Product>(s => s
            .Index(elasricsearchOptions.Value.DefaultIndex)
            .Sort(so => so
                .Field(f => f.TotalAmountSold, new FieldSort { Order = SortOrder.Desc }))
            .Size(10)
        );
        return sortedProducts.Documents;
    }

    public async Task<IEnumerable<Product>> GetNewProductsAsync()
    {
        await CheckIfIndexExists();
        var sortedProducts = await client.SearchAsync<Product>(s => s
            .Index(elasricsearchOptions.Value.DefaultIndex)
            .Sort(so => so
                .Field(f => f.CreatedDate, new FieldSort { Order = SortOrder.Desc }))
            .Size(10)
        );
        return sortedProducts.Documents;
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }

    private async Task CheckIfIndexExists()
    {
        if (!( client.Indices.Exists(elasricsearchOptions.Value.DefaultIndex)).Exists)
        {
           var result = await client.Indices.CreateAsync(elasricsearchOptions.Value.DefaultIndex);
        }
    }
}