using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize]
[Route("api/category")]
[ApiController]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody]CategoryDto categoryDto)
    {
        var category = await categoryService.CreateCategoryAsync(categoryDto);
        return Ok(category);
    }   

    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryDto categoryDto, Guid categoryId)
    {
        var category = await categoryService.UpdateCategoryAsync(categoryDto, categoryId);
        return Ok(category);
    }

    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategory(Guid categoryId)
    {
        var category = await categoryService.GetCategoryByIdAsync(categoryId);
        return Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        await categoryService.DeleteCategoriesAsync(categoryId);
        return NoContent();
    }
}
