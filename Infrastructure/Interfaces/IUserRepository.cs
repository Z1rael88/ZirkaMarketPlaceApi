using Domain.Models;

namespace Infrastructure.Interfaces;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
    Task<User> UpdateUserAsync(User user);
    Task<User> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task DeleteUserAsync(Guid userId);
}