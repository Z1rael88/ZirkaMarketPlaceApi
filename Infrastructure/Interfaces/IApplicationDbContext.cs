using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync();
}