using Domain.Models;
using Infrastructure.Data.Interceptors;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IApplicationUser user)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Product> Products { get; set; }

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new AuditableEntityInterceptor(user));
}