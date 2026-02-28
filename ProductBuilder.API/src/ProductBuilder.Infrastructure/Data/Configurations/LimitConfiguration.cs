using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class LimitConfiguration : IEntityTypeConfiguration<Limit>
{
    public void Configure(EntityTypeBuilder<Limit> builder)
    {
        builder.ToTable("limits");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id");
        builder.Property(l => l.CoverId).HasColumnName("cover_id");
        builder.Property(l => l.LimitType).HasColumnName("limit_type").HasMaxLength(50).IsRequired();
        builder.Property(l => l.MinAmount).HasColumnName("min_amount").HasPrecision(18, 2);
        builder.Property(l => l.MaxAmount).HasColumnName("max_amount").HasPrecision(18, 2);
        builder.Property(l => l.DefaultAmount).HasColumnName("default_amount").HasPrecision(18, 2);
        builder.Property(l => l.Currency).HasColumnName("currency").HasMaxLength(10);
        builder.Property(l => l.IsActive).HasColumnName("is_active");
        builder.HasOne(l => l.Cover).WithMany(c => c.Limits).HasForeignKey(l => l.CoverId).OnDelete(DeleteBehavior.Cascade);
    }
}
