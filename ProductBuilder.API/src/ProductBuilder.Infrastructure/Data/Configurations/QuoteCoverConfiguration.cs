using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class QuoteCoverConfiguration : IEntityTypeConfiguration<QuoteCover>
{
    public void Configure(EntityTypeBuilder<QuoteCover> builder)
    {
        builder.ToTable("quote_covers");
        builder.HasKey(qc => qc.Id);
        builder.Property(qc => qc.Id).HasColumnName("id");
        builder.Property(qc => qc.QuoteId).HasColumnName("quote_id");
        builder.Property(qc => qc.CoverId).HasColumnName("cover_id");
        builder.Property(qc => qc.IsSelected).HasColumnName("is_selected");
        builder.Property(qc => qc.SelectedLimit).HasColumnName("selected_limit").HasPrecision(18, 2);
        builder.Property(qc => qc.SelectedDeductible).HasColumnName("selected_deductible").HasPrecision(18, 2);
        builder.Property(qc => qc.CalculatedPremium).HasColumnName("calculated_premium").HasPrecision(18, 2);
        builder.Property(qc => qc.BasisValue).HasColumnName("basis_value").HasPrecision(18, 2);
        builder.HasOne(qc => qc.Quote).WithMany(q => q.QuoteCovers).HasForeignKey(qc => qc.QuoteId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(qc => qc.Cover).WithMany(c => c.QuoteCovers).HasForeignKey(qc => qc.CoverId);
    }
}
