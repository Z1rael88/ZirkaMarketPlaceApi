using Domain.Models;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductConfiguration(ProductValidationOptions productValidationOptions) : BaseEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.Name)
           .HasMaxLength(productValidationOptions.NameMaxLength)
            .IsRequired();
        builder.Property(p => p.Description)
           .HasMaxLength(productValidationOptions.DescriptionMaxLength)
            .IsRequired();
        builder.Property(p => p.AvailableAmount)
            .IsRequired();
        builder.Property(p => p.TotalAmountSold)
            .IsRequired();
        builder.Property(p => p.Rating)
            .IsRequired();
        builder.Property(p => p.Price)
            .IsRequired();
        builder.Property(p => p.PhotoUrl)
            .IsRequired();
    }
}