using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");
        builder.HasKey(p => p.Id);
        
        builder.HasOne(p => p.Buyer)
            .WithMany(u => u.PurchasesAsBuyer)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Seller)
            .WithMany(u => u.PurchasesAsSeller)
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Product)
            .WithMany(p => p.Purchases)
            .HasForeignKey(p => p.ProductId);

        builder.Property(p => p.Quantity)
            .IsRequired();

    }
}