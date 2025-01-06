using Application.Dtos;
using Application.Interfaces;
using Domain.Filters;
using Domain.Models;
using Elastic.Clients.Elasticsearch;
using FluentValidation;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Mapster;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class ProductService(
    IProductRepository productRepository,
    IFileStorageService fileStorageService,
    IValidator<ProductDto> productValidator)
    : IProductService
{
    public async Task<ProductResponseDto> CreateProductAsync(ProductDto productDto)
    {
        productValidator.ValidateAndThrow(productDto);
        var product = productDto.Adapt<Product>();
        product.TotalAmountSold = 0;
        product.PhotoUrl = await fileStorageService.UploadPhotoAsync(productDto.PhotoUrl, "product-photos");
        product.Status = product.AvailableAmount > 0 ? ProductStatus.Available : ProductStatus.OutOfStock;
        var createdProduct = await productRepository.CreateProductAsync(product);
        return createdProduct.Adapt<ProductResponseDto>();
    }

    public async Task<ProductResponseDto> UpdateProductAsync(ProductDto productDto, Guid productId)
    {
        productValidator.ValidateAndThrow(productDto);
        var existingProduct = await productRepository.GetProductByIdAsync(productId);
        productDto.Adapt(existingProduct);
        existingProduct.Status = existingProduct.AvailableAmount > 0 ? ProductStatus.Available : ProductStatus.OutOfStock;
        var updatedProduct = await productRepository.UpdateProductAsync(existingProduct);
        return updatedProduct.Adapt<ProductResponseDto>();
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(Guid productId)
    {
        var product = await productRepository.GetProductByIdAsync(productId);
        return product.Adapt<ProductResponseDto>();
    }

    public async Task UpdateRatingAsync(Guid productId, int rating)
    {
        var product = await productRepository.GetProductByIdAsync(productId);
        product.Ratings ??= [];
        product.Ratings?.Add(rating);
        var updatedRating = ReturnAverageRatingOfProduct(product.Ratings!);
        product.Rating = updatedRating;
        await productRepository.UpdateProductAsync(product);
    }

    public async Task<PaginatedResponse<ProductResponseDto>> GetAllPaginatedProductsAsync(int pageNumber, int pageSize,
        ProductFilter? filter = null)
    {
        var product = await productRepository.GetAllPaginatedProductsAsync(pageNumber, pageSize, filter);
        return product.Adapt<PaginatedResponse<ProductResponseDto>>();
    }

    public async Task<IEnumerable<ProductResponseDto>> GetBestSellersAsync()
    {
        var sortedProducts = await productRepository.GetBestSellersAsync();
        return sortedProducts.Adapt<IEnumerable<ProductResponseDto>>();
    }

    public async Task<IEnumerable<ProductResponseDto>> GetNewProductsAsync()
    {
        var sortedProducts = await productRepository.GetNewProductsAsync();
        return sortedProducts.Adapt<IEnumerable<ProductResponseDto>>();
    }

    public async Task DeleteProductAsync(Guid productId)
    {
        await productRepository.DeleteProductAsync(productId);
    }

    private int ReturnAverageRatingOfProduct(List<int> ratings)
    {
        if (ratings.Count > 0)
        {
            var sum = ratings.Sum();
            var avg = sum / ratings.Count;
            return avg;
        }

        return 0;
    }
    public async Task<IEnumerable<ProductDto>> GetProductsByStatusAsync(ProductStatus status)
    {
        var products = await productRepository.GetProductsByStatusAsync(status);
        return products.Adapt<IEnumerable<ProductDto>>();
    }
}