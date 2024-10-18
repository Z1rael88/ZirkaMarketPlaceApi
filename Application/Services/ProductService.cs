using Application.Dtos;
using Application.Interfaces;
using Domain.Filters;
using Domain.Models;
using Infrastructure.Interfaces;
using Mapster;

namespace Application.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto productDto)
    {
        var product = productDto.Adapt<Product>();
        var createdProduct = await productRepository.CreateProductAsync(product);
        createdProduct.TotalAmountSold = 0;
        return createdProduct.Adapt<ProductResponseDto>();
    }

    public async Task<ProductResponseDto> UpdateProductAsync(ProductDto productDto,Guid productId)
    {
        var existingProduct = await productRepository.GetProductByIdAsync(productId);
        productDto.Adapt(existingProduct);
        var updatedProduct = await productRepository.UpdateProductAsync(existingProduct);
        return updatedProduct.Adapt<ProductResponseDto>();
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(Guid productId)
    {
        var product = await productRepository.GetProductByIdAsync(productId);
        return product.Adapt<ProductResponseDto>();
    }

    public async Task<PaginatedResponse<ProductResponseDto>> GetAllPaginatedProductsAsync(int pageNumber,int pageSize,ProductFilter? filter = null)
    {
        var product = await productRepository.GetAllPaginatedProductsAsync(pageNumber,pageSize,filter);
        return product.Adapt<PaginatedResponse<ProductResponseDto>>();
    }

    public async Task DeleteProductAsync(Guid productId)
    {
        await productRepository.DeleteProductAsync(productId);
    }
}