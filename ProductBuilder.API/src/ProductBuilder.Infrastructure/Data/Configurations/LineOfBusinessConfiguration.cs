using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class LineOfBusinessConfiguration : IEntityTypeConfiguration<LineOfBusiness>
{
    public void Configure(EntityTypeBuilder<LineOfBusiness> builder)
    {
        builder.ToTable("lines_of_business");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id");
        builder.Property(l => l.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(l => l.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.Property(l => l.Description).HasColumnName("description");
        builder.Property(l => l.IsActive).HasColumnName("is_active");
        builder.Property(l => l.CreatedAt).HasColumnName("created_at");
        builder.Property(l => l.UpdatedAt).HasColumnName("updated_at");
        builder.HasIndex(l => l.Code).IsUnique();
    }
}
