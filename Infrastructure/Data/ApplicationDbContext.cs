  using Domain.Models;
using Infrastructure.Data.Configurations;
using Infrastructure.Data.Interceptors;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;

namespace Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IApplicationUser user)
        : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options),IApplicationDbContext
    {
        public EntityEntry Entry(object entity) => base.Entry(entity);
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        
        public DbSet<Purchase> Purchases { get; set; } 

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ApplyConfigurations(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new AuditableEntityInterceptor(user));
        }
        private void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var categoryValidationOptions = this.GetService<IOptions<CategoryValidationOptions>>().Value;
            var productValidationOptions = this.GetService<IOptions<ProductValidationOptions>>().Value;

            modelBuilder
                .ApplyConfiguration(new UserConfiguration())
                .ApplyConfiguration(new CategoryConfiguration(categoryValidationOptions))
                .ApplyConfiguration(new ProductConfiguration(productValidationOptions));
                .ApplyConfiguration(new ProductConfiguration(productValidationOptions))
                .ApplyConfiguration(new PurchaseConfiguration());
        }
    }
}