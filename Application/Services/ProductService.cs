using Application.Dtos;
using Application.Interfaces;
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

    public async Task<ProductResponseDto> UpdateProductAsync(ProductDto productDto, Guid productId)
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

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        var product = await productRepository.GetProductsAsync();
        return product.Adapt<IEnumerable<ProductResponseDto>>();
    }

    public async Task DeleteProductAsync(Guid productId)
    {
        await productRepository.DeleteProductAsync(productId);
    }
}