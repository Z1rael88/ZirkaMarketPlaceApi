using Application.Dtos;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace Presentation.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterUserDto registerUserDto)
    {
        var user = await userService.RegisterUserAsync(registerUserDto);
        return Ok(user);
    }
   [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginDto loginDto)
    {
        var tokens = await userService.LoginAsync(loginDto);
        return Ok(tokens);
    }
    [HttpPost("refreshtoken")]
    public async Task<IActionResult> RefreshToken(string accessToken)
    {
        var tokens = await userService.RefreshTokenAsync(accessToken);
        return Ok(tokens);
    }
    [Authorize]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(BaseUserDto baseUserDto,Guid userId)
    {
        var user = await userService.UpdateUserAsync(baseUserDto,userId);
        return Ok(user);
    }
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        var user = await userService.GetUserAsync(userId);
        return Ok(user);
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }
    [AuthorizeWithRoles(Role.SystemAdministrator)]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
       await userService.DeleteUserAsync(userId);
       return NoContent();
    }
}