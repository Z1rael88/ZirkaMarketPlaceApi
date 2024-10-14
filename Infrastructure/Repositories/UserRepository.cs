using Domain.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(IApplicationDbContext dbContext) : IUserRepository
{
    public async Task<User> AddUserAsync(User user)
    {
        var createdUser = await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return createdUser.Entity;
    }
    public async Task<User> UpdateUserAsync(User user)
    {
        var userToUpdate = await GetUserByIdAsync(user.Id);
        userToUpdate.FirstName = user.FirstName;
        userToUpdate.LastName = user.LastName;
        userToUpdate.Age = user.Age;
        userToUpdate.Email = user.Email;
        await dbContext.SaveChangesAsync();
        return userToUpdate;
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new ArgumentException($"User with Id: {userId} not found");
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await dbContext.Users.ToListAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await GetUserByIdAsync(userId);
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
    }
}