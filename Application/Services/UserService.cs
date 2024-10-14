using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class UserService(
    IUserRepository userRepository,
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IOptions<JwtOptions> jwtOptions) : IUserService
{
    public async Task<BaseUserDto> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var roleName = registerUserDto.Role.ToString();
        var newUser = registerUserDto.Adapt<User>();
        await CreateUserAndAssignRoleAsync(newUser, registerUserDto.Password, roleName);
        var userDto = newUser.Adapt<BaseUserDto>();
        userDto.Role = registerUserDto.Role;
        return userDto;
    }

    public async Task<BaseUserDto> UpdateUserAsync(BaseUserDto baseUserDto, Guid userId)
    {
        var user = await GetUserByIdAsync(userId);
        await UpdateUserRoleAsync(user, baseUserDto.Role.ToString());
        var userToUpdate = baseUserDto.Adapt<User>();
        userToUpdate.Id = userId;
        var updatedUser = await userRepository.UpdateUserAsync(userToUpdate);
        var updatedUserDto = updatedUser.Adapt<BaseUserDto>();
        updatedUserDto.Role = baseUserDto.Role;
        updatedUserDto.UserName = baseUserDto.UserName;
        return updatedUserDto;
    }

    public async Task<BaseUserDto> GetUserAsync(Guid userId)
    {
        var user = await userRepository.GetUserByIdAsync(userId);
        
        var roleString = (await userManager.GetRolesAsync(user)).SingleOrDefault();
        var userRole = Enum.TryParse<Role>(roleString, out var parsedRole) ? parsedRole : default;
        
        var userDto = user.Adapt<BaseUserDto>();
        userDto.Role = userRole;
        if (user.UserName != null) userDto.UserName = user.UserName;
        return userDto;
    }

    public async Task<IEnumerable<BaseUserDto>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllUsersAsync();
        var userDtos = new List<BaseUserDto>();

        foreach (var user in users)
        {
            var roleString = (await userManager.GetRolesAsync(user)).SingleOrDefault();
            var userRole = Enum.TryParse<Role>(roleString, out var parsedRole) ? parsedRole : default;

            var userDto = user.Adapt<BaseUserDto>();
            userDto.Role = userRole;
            userDtos.Add(userDto);
        }

        return userDtos;
    }


    public async Task DeleteUserAsync(Guid userId)
    {
        await userRepository.DeleteUserAsync(userId);
    }

    public async Task<TokensDto> RefreshTokenAsync(string refreshToken)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        TokenValidationParameters validationParameters = new()
        {
            ValidIssuer = jwtOptions.Value.Issuer,
            ValidAudience = jwtOptions.Value.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey)),
        };
        var principal = tokenHandler.ValidateToken(
            refreshToken,
            validationParameters,
            out SecurityToken? _);
        var userProfileIdClaim = principal.FindFirst(ClaimTypes.Sid)?.Value;
        if (userProfileIdClaim == null)
        {
            throw new SecurityTokenException("Invalid token");
        }

        if (!Guid.TryParse(userProfileIdClaim, out Guid userProfileId))
        {
            throw new SecurityTokenException("Invalid user profile ID format");
        }

        var user = await GetApplicationUserAsync(userProfileId);
        var role = await GetRoleByUserAsync(user);
        var newAcсessToken = GenerateAccessToken(userProfileId, role);
        var newRefreshToken = GenerateRefreshToken(userProfileId);
        return CreateTokensDto(newAcсessToken, newRefreshToken);
    }

    public async Task<TokensDto> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.UserName);
        if (user == null)
        {
            throw new ArgumentException($"User with userName {loginDto.UserName} not found");
        }

        bool isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid password");
        }

        var role = await GetRoleByUserAsync(user);
        var accessToken = GenerateAccessToken(user.Id, role);
        var refreshToken = GenerateRefreshToken(user.Id);
        return CreateTokensDto(accessToken, refreshToken);
    }

    private string GenerateAccessToken(Guid userProfileId, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, userProfileId.ToString()),
            new(ClaimTypes.Role, role),
        };
        var expirationTime = DateTime.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenExpiryMinutes);
        return GenerateToken(claims, expirationTime);
    }

    private string GenerateRefreshToken(Guid userProfileId)
    {
        var claims = new List<Claim> { new(ClaimTypes.Sid, userProfileId.ToString()) };
        var expirationTime = DateTime.UtcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpiryMinutes);
        return GenerateToken(claims, expirationTime);
    }

    private string GenerateToken(
        IEnumerable<Claim> claims,
        DateTime expiration)
    {
        var jwtOptionsValue = jwtOptions.Value;

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptionsValue.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            jwtOptionsValue.Issuer,
            jwtOptionsValue.Audience,
            claims,
            DateTime.UtcNow,
            expiration,
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GetRoleByUserAsync(User user)
    {
        var roles = await userManager.GetRolesAsync(user);
        return roles.Single();
    }

    private async Task ValidateRoleAsync(Role role)
    {
        if (!await roleManager.RoleExistsAsync(role.ToString()))
        {
            throw new ArgumentException(
                $"That Role with name {role.ToString()} is not found, the Role should be a specified enum value ");
        }
    }

    private async Task<User> GetApplicationUserAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new ArgumentException($"User with profile id {userId} not found");
        }

        return user;
    }

    private async Task CreateUserAndAssignRoleAsync(User user, string password, string roleName)
    {
        var userCreationResult = await userManager.CreateAsync(user, password);
        if (!userCreationResult.Succeeded)
        {
            throw new IdentityException("User creation failed", userCreationResult.Errors);
        }

        var roleAssignmentResult = await userManager.AddToRoleAsync(user, roleName);
        if (!roleAssignmentResult.Succeeded)
        {
            throw new IdentityException("Role assignment failed", roleAssignmentResult.Errors);
        }
    }

    private TokensDto CreateTokensDto(string accessToken, string refreshToken)
    {
        var refreshTokenExpiryTime = jwtOptions.Value.RefreshTokenExpiryMinutes;
        var accessTokenExpiryTime = jwtOptions.Value.AccessTokenExpiryMinutes;
        var accessTokenExpirationDate = DateTime.UtcNow.AddMinutes(accessTokenExpiryTime);
        var refreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(refreshTokenExpiryTime);
        return new TokensDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpirationDate = refreshTokenExpirationDate,
            AccessTokenExpirationDate = accessTokenExpirationDate,
        };
    }

    private async Task UpdateUserRoleAsync(User user, string roleName)
    {
        var existingUserRoleName = (await userManager.GetRolesAsync(user)).Single();
        if (existingUserRoleName != roleName)
        {
            var removeRolesResult = await userManager.RemoveFromRoleAsync(user, existingUserRoleName);
            if (!removeRolesResult.Succeeded)
            {
                throw new IdentityException("Role deletion failed", removeRolesResult.Errors);
            }

            var addRoleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!addRoleResult.Succeeded)
            {
                throw new IdentityException("Role add failed", addRoleResult.Errors);
            }
        }
    }

    private async Task<User> GetUserByIdAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new ArgumentException($"User with profile id {userId} not found");
        }

        return user;
    }
}