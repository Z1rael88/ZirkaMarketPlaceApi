using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dtos;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Enums;
using Domain.Models;
using Google.Apis.Auth;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Application.Services;

public class UserService(
    IUserRepository userRepository,
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IOptions<JwtOptions> jwtOptions,
    IOptions<GoogleOptions> googleOptions,
    IHttpContextAccessor httpContextAccessor) : IUserService
{
    public async Task<BaseUserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var roleName = registerUserDto.Role.ToString();
        await ValidateRoleAsync(registerUserDto.Role);
        var newUser = registerUserDto.Adapt<User>();
        await CreateUserAndAssignRoleAsync(newUser, registerUserDto.Password, roleName);
        var userDto = newUser.Adapt<BaseUserResponseDto>();
        userDto.Role = registerUserDto.Role;
        return userDto;
    }

    public async Task<TokensDto> GenerateJwtTokensForGoogleUser(GoogleJsonWebSignature.Payload payload)
    {
        string role = null;
        string roleName = "Buyer";
        var user = await userRepository.GetUserByGoogleIdAsync(payload.Subject);
        if (user == null)
        {
            user = new User
            {
                GoogleId = payload.Subject,
                FirstName = payload.GivenName,
                LastName = string.IsNullOrEmpty(payload.FamilyName) ? payload.GivenName : payload.FamilyName,
                Email = payload.Email,
                UserName = $"{payload.GivenName}{payload.FamilyName?.Substring(0, 2)}",
                SecurityStamp = Guid.NewGuid().ToString(), 
                EmailConfirmed = true, 
            };
            await userRepository.AddUserAsync(user);
            await userManager.AddToRoleAsync(user, roleName);
        }

        if (!await userManager.IsInRoleAsync(user,roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
        role = await GetRoleByUserAsync(user);
        var accessToken = GenerateAccessToken(user.Id, role);
        var refreshToken = GenerateRefreshToken(user.Id);

        return CreateTokensDto(accessToken, refreshToken, user.Id, role);
    }

    public async Task<BaseUserResponseDto> UpdateUserAsync(BaseUserDto baseUserDto, Guid userId)
    {
        var user = await GetUserByIdAsync(userId);
        await UpdateUserRoleAsync(user, baseUserDto.Role.ToString());
        var userToUpdate = baseUserDto.Adapt<User>();
        userToUpdate.Id = userId;
        var updatedUser = await userRepository.UpdateUserAsync(userToUpdate);
        var updatedUserDto = updatedUser.Adapt<BaseUserResponseDto>();
        updatedUserDto.Role = baseUserDto.Role;
        updatedUserDto.UserName = baseUserDto.UserName;
        return updatedUserDto;
    }

    public async Task<BaseUserResponseDto> GetUserAsync(Guid userId)
    {
        var user = await userRepository.GetUserByIdAsync(userId);

        var roleString = (await userManager.GetRolesAsync(user)).SingleOrDefault();
        var userRole = Enum.TryParse<Role>(roleString, out var parsedRole) ? parsedRole : default;

        var userDto = user.Adapt<BaseUserResponseDto>();
        userDto.Role = userRole;
        if (user.UserName != null) userDto.UserName = user.UserName;
        return userDto;
    }

    public async Task<IEnumerable<BaseUserResponseDto>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllUsersAsync();
        var userDtos = new List<BaseUserResponseDto>();

        foreach (var user in users)
        {
            var roleString = (await userManager.GetRolesAsync(user)).SingleOrDefault();
            var userRole = Enum.TryParse<Role>(roleString, out var parsedRole) ? parsedRole : default;

            var userDto = user.Adapt<BaseUserResponseDto>();
            userDto.Role = userRole;
            userDtos.Add(userDto);
        }

        return userDtos;
    }


    public async Task DeleteUserAsync(Guid userId)
    {
        await userRepository.DeleteUserAsync(userId);
    }

    public void Logout()
    {
        var accessTokenCookie = httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];
        var refreshTokenCookie = httpContextAccessor.HttpContext?.Request.Cookies["RefreshToken"];

        if (accessTokenCookie != null && refreshTokenCookie != null)
        {
            httpContextAccessor.HttpContext!.Response.Cookies.Delete("AccessToken");
            httpContextAccessor.HttpContext!.Response.Cookies.Delete("RefreshToken");
        }

    }

    public async Task<TokensDto> RefreshTokenAsync()
    {
        var refreshToken = httpContextAccessor.HttpContext.Request.Cookies["RefreshToken"];
        Console.WriteLine(refreshToken);
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

        WriteTokenToCookies("AccessToken", newAcсessToken, jwtOptions.Value.AccessTokenExpiryMinutes);
        WriteTokenToCookies("RefreshToken", newRefreshToken, jwtOptions.Value.RefreshTokenExpiryMinutes);

        return CreateTokensDto(newAcсessToken, newRefreshToken, user.Id, role);
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

        WriteTokenToCookies("AccessToken", accessToken, jwtOptions.Value.AccessTokenExpiryMinutes);
        WriteTokenToCookies("RefreshToken", refreshToken, jwtOptions.Value.RefreshTokenExpiryMinutes);
        return CreateTokensDto(accessToken, refreshToken, user.Id, role);
    }

    public async Task LoginWithGoogleAsync(string code)
    {
        var googleOptionsValue = googleOptions.Value;

        var clientId = googleOptionsValue.ClientId;
        var clientSecret = googleOptionsValue.ClientSecret;
        var redirectUri = "http://zirkamarketplace.shop/api/users/google-signin";

        using var client = new HttpClient();
        var tokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUri },
                { "grant_type", "authorization_code" }
            }));

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
        var googleTokens = JsonConvert.DeserializeObject<GoogleTokenResponse>(tokenJson);

        var payload = await GoogleJsonWebSignature.ValidateAsync(googleTokens.IdToken);

        await GenerateJwtTokensForGoogleUser(payload);
    }

    public string CreateGoogleUrl()
    {
        var clientId = googleOptions.Value.ClientId;
        var redirectUri = "http://zirkamarketplace.shop/api/users/google-signin";
        var scope = "openid email profile";
        var state = Guid.NewGuid().ToString();

        return $"https://accounts.google.com/o/oauth2/auth" +
                            $"?client_id={clientId}" +
                            $"&redirect_uri={redirectUri}" +
                            $"&response_type=code" +
                            $"&scope={scope}" +
                            $"&state={state}";
    }

    private void WriteTokenToCookies(string key, string token, int expiryMinutes)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };

        httpContextAccessor.HttpContext!.Response.Cookies.Append(key, token, cookieOptions);
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

        if (role == Role.SystemAdministrator)
        {
            throw new ArgumentException(
                "You cannot register as a System Administrator");
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

    private TokensDto CreateTokensDto(string accessToken, string refreshToken, Guid userId, string role)
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
            UserId = userId,
            Role = role
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