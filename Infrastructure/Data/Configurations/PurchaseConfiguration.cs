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
             .OnDelete(DeleteBehavior.NoAction);
         
         builder.HasOne(p => p.Seller)
             .WithMany(u => u.PurchasesAsSeller)
             .HasForeignKey(p => p.SellerId)
             .OnDelete(DeleteBehavior.Restrict);
 
         builder.HasOne(p => p.Product)
             .WithOne(p => p.Purchase)
             .HasForeignKey<Purchase>(p => p.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
 
         builder.Property(p => p.Quantity)
             .IsRequired();
         
         builder.Property(p => p.Status)
             .IsRequired();
     }
 }