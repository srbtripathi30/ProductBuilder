using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class PremiumConfiguration : IEntityTypeConfiguration<Premium>
{
    public void Configure(EntityTypeBuilder<Premium> builder)
    {
        builder.ToTable("premiums");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.CoverId).HasColumnName("cover_id");
        builder.Property(p => p.PremiumType).HasColumnName("premium_type").HasMaxLength(50).IsRequired();
        builder.Property(p => p.BaseRate).HasColumnName("base_rate").HasPrecision(10, 6);
        builder.Property(p => p.FlatAmount).HasColumnName("flat_amount").HasPrecision(18, 2);
        builder.Property(p => p.MinPremium).HasColumnName("min_premium").HasPrecision(18, 2);
        builder.Property(p => p.CalculationBasis).HasColumnName("calculation_basis").HasMaxLength(100);
        builder.Property(p => p.Currency).HasColumnName("currency").HasMaxLength(10);
        builder.Property(p => p.IsActive).HasColumnName("is_active");
        builder.HasOne(p => p.Cover).WithMany(c => c.Premiums).HasForeignKey(p => p.CoverId).OnDelete(DeleteBehavior.Cascade);
    }
}
