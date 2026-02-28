using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class DeductibleConfiguration : IEntityTypeConfiguration<Deductible>
{
    public void Configure(EntityTypeBuilder<Deductible> builder)
    {
        builder.ToTable("deductibles");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");
        builder.Property(d => d.CoverId).HasColumnName("cover_id");
        builder.Property(d => d.DeductibleType).HasColumnName("deductible_type").HasMaxLength(50).IsRequired();
        builder.Property(d => d.MinAmount).HasColumnName("min_amount").HasPrecision(18, 2);
        builder.Property(d => d.MaxAmount).HasColumnName("max_amount").HasPrecision(18, 2);
        builder.Property(d => d.DefaultAmount).HasColumnName("default_amount").HasPrecision(18, 2);
        builder.Property(d => d.Currency).HasColumnName("currency").HasMaxLength(10);
        builder.Property(d => d.IsActive).HasColumnName("is_active");
        builder.HasOne(d => d.Cover).WithMany(c => c.Deductibles).HasForeignKey(d => d.CoverId).OnDelete(DeleteBehavior.Cascade);
    }
}
