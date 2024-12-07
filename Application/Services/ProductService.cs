using System.Text;
using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Domain.Filters;
using Domain.Models;
using FluentValidation;
using Infrastructure.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class ProductService(
    IProductRepository productRepository,
    IFileStorageService fileStorageService,
    IValidator<ProductDto> productValidator) : IProductService
{
    public async Task<ProductResponseDto> CreateProductAsync(ProductDto productDto)
    {
        productValidator.ValidateAndThrow(productDto);
        var  photoUrlBase64 = await ConverterFromIFormFileToString.ConvertIFormFileToBase64Async(productDto.PhotoUrl);
        var product = productDto.Adapt<Product>();
        product.PhotoUrl = photoUrlBase64; 
        product.TotalAmountSold = 0;
        var createdProduct = await productRepository.CreateProductAsync(product);
        createdProduct.PhotoUrl = await fileStorageService.UploadPhotoAsync(productDto.PhotoUrl, "product-photos");
        return createdProduct.Adapt<ProductResponseDto>();
    }

    public async Task<ProductResponseDto> UpdateProductAsync(ProductDto productDto, Guid productId)
    {
        productValidator.ValidateAndThrow(productDto);
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

    private IFormFile ConvertStringToIFormFile(string content, string fileName = "file.txt",
        string contentType = "text/plain")
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(content);

        var stream = new MemoryStream(byteArray);

        IFormFile formFile = new FormFile(stream, 0, byteArray.Length, "file", fileName)
        {
            ContentType = contentType
        };
        return formFile;
    }
}