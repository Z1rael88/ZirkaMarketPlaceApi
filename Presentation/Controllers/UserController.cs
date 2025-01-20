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
    
    [HttpGet("googleLogin")]
    public IActionResult GetGoogleAuthUrl()
    {
        var googleAuthUrl = userService.CreateGoogleUrl();
        return Ok(new { AuthUrl = googleAuthUrl });
    }
    
    [HttpGet("google-signin")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        await userService.LoginWithGoogleAsync(code);
        return Redirect("https://zirka-market-place-ui.vercel.app/");
    }


    [HttpPost("logout")]
    public IActionResult Logout()
    {
        userService.Logout();
    
        return Ok("User logged out");
    }

    [HttpPost("refreshtoken")]
    public async Task<IActionResult> RefreshToken()
    {
        var tokens = await userService.RefreshTokenAsync();
        return Ok(tokens);
    }

    [Authorize]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(BaseUserDto baseUserDto, Guid userId)
    {
        var user = await userService.UpdateUserAsync(baseUserDto, userId);
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