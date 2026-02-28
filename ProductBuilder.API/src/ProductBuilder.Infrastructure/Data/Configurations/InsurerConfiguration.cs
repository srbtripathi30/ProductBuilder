using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class InsurerConfiguration : IEntityTypeConfiguration<Insurer>
{
    public void Configure(EntityTypeBuilder<Insurer> builder)
    {
        builder.ToTable("insurers");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(i => i.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.Property(i => i.LicenseNo).HasColumnName("license_no").HasMaxLength(100);
        builder.Property(i => i.Address).HasColumnName("address");
        builder.Property(i => i.Phone).HasColumnName("phone").HasMaxLength(50);
        builder.Property(i => i.Email).HasColumnName("email").HasMaxLength(255);
        builder.Property(i => i.IsActive).HasColumnName("is_active");
        builder.Property(i => i.CreatedAt).HasColumnName("created_at");
        builder.HasIndex(i => i.Code).IsUnique();
    }
}
