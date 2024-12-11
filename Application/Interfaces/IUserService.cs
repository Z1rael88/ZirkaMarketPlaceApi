using Application.Dtos;

namespace Application.Interfaces;

public interface IUserService
{
    Task<BaseUserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<TokensDto> LoginAsync(LoginDto loginDto);
    Task<TokensDto> RefreshTokenAsync(string refreshToken);
    Task<BaseUserResponseDto> UpdateUserAsync(BaseUserDto baseUserDto,Guid userId);
    Task<BaseUserResponseDto> GetUserAsync(Guid userId);
    Task<IEnumerable<BaseUserResponseDto>> GetAllUsersAsync();
    Task DeleteUserAsync(Guid userId);



}