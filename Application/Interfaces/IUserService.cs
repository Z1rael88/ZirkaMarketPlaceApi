using Application.Dtos;
using Google.Apis.Auth;

namespace Application.Interfaces;

public interface IUserService
{
    Task<BaseUserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<TokensDto> LoginAsync(LoginDto loginDto);
    Task<TokensDto> RefreshTokenAsync();
    Task<BaseUserResponseDto> UpdateUserAsync(BaseUserDto baseUserDto,Guid userId);
    Task<BaseUserResponseDto> GetUserAsync(Guid userId);
    Task<IEnumerable<BaseUserResponseDto>> GetAllUsersAsync();
    Task DeleteUserAsync(Guid userId);
    void Logout();
    Task LoginWithGoogleAsync(string code);
    string CreateGoogleUrl();


}