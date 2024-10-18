using Application.Dtos;
using Application.Interfaces;
using Domain.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
[Route("api/products")]
[ApiController]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        var product = await productService.CreateProductAsync(productDto);
        return Ok(product);
    }

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
        var product = await productService.GetAllPaginatedProductsAsync(pageNumber, pageSize, filter);
        return Ok(product);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        await productService.DeleteProductAsync(productId);
        return NoContent();
    }
}