using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T>
        where T : class, IBaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.CreatedDate).IsRequired();
            builder.Property(entity => entity.CreatedBy).IsRequired();
            builder.Property(entity => entity.UpdatedDate).IsRequired();
            builder.Property(entity => entity.UpdatedBy).IsRequired();
            builder.Property(entity => entity.IsDeleted).IsRequired();
        }
    }
}