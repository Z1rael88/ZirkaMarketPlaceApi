using Application.Dtos;
using Application.Interfaces;
using Domain.Enums;
using Domain.Filters;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace Presentation.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController(IProductService productService) : ControllerBase
{
    [AuthorizeWithRoles(Role.Seller,Role.SystemAdministrator)]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromForm] ProductDto productDto)
    {
        var product = await productService.CreateProductAsync(productDto);
        return Ok(product);
    }
    [AuthorizeWithRoles(Role.Seller,Role.SystemAdministrator)]
    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProduct([FromBody] ProductDto productDto, Guid productId)
    {
        var product = await productService.UpdateProductAsync(productDto, productId);
        return Ok(product);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProduct(Guid productId)
    {
        var product = await productService.GetProductByIdAsync(productId);
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 10,
        [FromQuery] ProductFilter? filter = null)
    {
        var products = await productService.GetAllPaginatedProductsAsync(pageNumber, pageSize, filter);
        return Ok(products);
    }
    [HttpGet("bestsellers")]
    public async Task<IActionResult> GetBestSellers()
    {
        var products = await productService.GetBestSellersAsync();
        return Ok(products);
    }
    [HttpGet("new-products")]
    public async Task<IActionResult> GetNewProducts()
    {
        var products = await productService.GetNewProductsAsync();
        return Ok(products);
    }
    [AuthorizeWithRoles(Role.Seller,Role.SystemAdministrator)]
    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        await productService.DeleteProductAsync(productId);
        return NoContent();
    }
    [AuthorizeWithRoles(Role.Buyer,Role.SystemAdministrator)]
    [HttpPatch]
    public async Task<IActionResult> UpdateRating(Guid productId,int rating)
    {
        await productService.UpdateRatingAsync(productId, rating);
        return Ok();
    }
}