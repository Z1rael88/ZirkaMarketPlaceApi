using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductConfiguration : BaseEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        builder.Property(p => p.Name)
            .IsRequired();
        builder.Property(p => p.Description)
            .IsRequired();
        builder.Property(p => p.AvailableAmount)
            .IsRequired();
        builder.Property(p => p.TotalAmountSold)
            .IsRequired();
        builder.Property(p => p.Rating)
            .IsRequired();
        builder.Property(p => p.PhotoUrl)
            .IsRequired();
    }
}