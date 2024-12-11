using Domain.Models;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CategoryConfiguration(CategoryValidationOptions categoryValidationOptions) : BaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);
        builder.Property(c => c.Name)
         .HasMaxLength(categoryValidationOptions.NameMaxLength)
            .IsRequired();
        builder.Property(c => c.Description)
         .HasMaxLength(categoryValidationOptions.DescriptionMaxLength)
            .IsRequired();
        builder.Property(c => c.PhotoUrl)
            .IsRequired();
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);
    }
}

