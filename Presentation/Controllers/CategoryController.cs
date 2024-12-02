using Application.Dtos;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace Presentation.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [AuthorizeWithRoles(Role.SystemAdministrator)]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody]CategoryDto categoryDto)
    {
        var category = await categoryService.CreateCategoryAsync(categoryDto);
        return Ok(category);
    }   
    [AuthorizeWithRoles(Role.SystemAdministrator)]
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

    [AuthorizeWithRoles(Role.SystemAdministrator)]
    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        await categoryService.DeleteCategoriesAsync(categoryId);
        return NoContent();
    }
}
