using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UserConfiguration
        : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.Property(a => a.Email)
                .IsRequired();
            builder.HasIndex(a => a.Email)
                .IsUnique();
            builder.Property(a => a.GoogleId);
            builder.Property(a => a.UserName)
                .IsRequired();
            builder.HasIndex(a => a.UserName)
                .IsUnique();
            builder.HasMany(p => p.Products)
                .WithOne(u=>u.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(u => u.PurchasesAsBuyer)
                .WithOne(p => p.Buyer)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.HasMany(u => u.PurchasesAsSeller)
                .WithOne(p => p.Seller)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}