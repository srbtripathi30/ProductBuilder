using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.LobId).HasColumnName("lob_id");
        builder.Property(p => p.InsurerId).HasColumnName("insurer_id");
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(p => p.Code).HasColumnName("code").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasColumnName("description");
        builder.Property(p => p.Version).HasColumnName("version").HasMaxLength(20).IsRequired();
        builder.Property(p => p.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(p => p.EffectiveDate).HasColumnName("effective_date");
        builder.Property(p => p.ExpiryDate).HasColumnName("expiry_date");
        builder.Property(p => p.CreatedBy).HasColumnName("created_by");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasOne(p => p.Lob).WithMany(l => l.Products).HasForeignKey(p => p.LobId);
        builder.HasOne(p => p.Insurer).WithMany(i => i.Products).HasForeignKey(p => p.InsurerId);
        builder.HasOne(p => p.Creator).WithMany().HasForeignKey(p => p.CreatedBy);
    }
}
