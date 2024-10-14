using Domain.Models;
using Infrastructure.Data.Interceptors;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IApplicationUser user)
        : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options),IApplicationDbContext
    {
        public EntityEntry Entry(object entity) => base.Entry(entity);
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new AuditableEntityInterceptor(user));
        }
    }
}