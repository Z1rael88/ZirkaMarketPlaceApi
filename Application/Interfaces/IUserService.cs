using Application.Dtos;

namespace Application.Interfaces;

public interface IUserService
{
    Task<BaseUserDto> RegisterUserAsync(RegisterUserDto registerUserDto);
    Task<TokensDto> LoginAsync(LoginDto loginDto);
    Task<TokensDto> RefreshTokenAsync(string refreshToken);
    Task<BaseUserDto> UpdateUserAsync(BaseUserDto baseUserDto,Guid userId);
    Task<BaseUserDto> GetUserAsync(Guid userId);
    Task<IEnumerable<BaseUserDto>> GetAllUsersAsync();
    Task DeleteUserAsync(Guid userId);



}