using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CategoryConfiguration : BaseEntityConfiguration<Category>
{
      public override void Configure(EntityTypeBuilder<Category> builder)
      {
           base.Configure(builder);
           builder.Property(c => c.Name)
               .IsRequired();
           builder.Property(c => c.Description)
               .IsRequired();
           builder.Property(c => c.PhotoUrl)
               .IsRequired();
      }
}

