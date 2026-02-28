using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class ModifierConfiguration : IEntityTypeConfiguration<Modifier>
{
    public void Configure(EntityTypeBuilder<Modifier> builder)
    {
        builder.ToTable("modifiers");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");
        builder.Property(m => m.CoverId).HasColumnName("cover_id");
        builder.Property(m => m.ProductId).HasColumnName("product_id");
        builder.Property(m => m.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(m => m.Code).HasColumnName("code").HasMaxLength(100).IsRequired();
        builder.Property(m => m.ModifierType).HasColumnName("modifier_type").HasMaxLength(50).IsRequired();
        builder.Property(m => m.ValueType).HasColumnName("value_type").HasMaxLength(50).IsRequired();
        builder.Property(m => m.MinValue).HasColumnName("min_value").HasPrecision(10, 4);
        builder.Property(m => m.MaxValue).HasColumnName("max_value").HasPrecision(10, 4);
        builder.Property(m => m.DefaultValue).HasColumnName("default_value").HasPrecision(10, 4);
        builder.Property(m => m.IsMandatory).HasColumnName("is_mandatory");
        builder.Property(m => m.Description).HasColumnName("description");
        builder.Property(m => m.IsActive).HasColumnName("is_active");
        builder.HasOne(m => m.Cover).WithMany(c => c.Modifiers).HasForeignKey(m => m.CoverId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(m => m.Product).WithMany(p => p.Modifiers).HasForeignKey(m => m.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}
