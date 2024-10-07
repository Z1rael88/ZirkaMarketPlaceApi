using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
    {
        var product = await productService.CreateProductAsync(productDto);
        return Ok(product);
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProduct([FromBody] ProductDto productDto,Guid productId)
    {
        var product = await productService.UpdateProductAsync(productDto,productId);
        return Ok(product);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProduct(Guid productId)
    {
        var product = await productService.GetProductByIdAsync(productId);
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var product = await productService.GetAllProductsAsync();
        return Ok(product);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        await productService.DeleteProductAsync(productId);
        return NoContent();
    }
}