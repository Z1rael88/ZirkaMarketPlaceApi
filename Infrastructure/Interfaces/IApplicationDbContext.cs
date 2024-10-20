using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<User> Users { get; }
    DbSet<Category> Categories { get; }
    EntityEntry Entry(object entity);

    Task<int> SaveChangesAsync();
}