using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class QuoteModifierConfiguration : IEntityTypeConfiguration<QuoteModifier>
{
    public void Configure(EntityTypeBuilder<QuoteModifier> builder)
    {
        builder.ToTable("quote_modifiers");
        builder.HasKey(qm => qm.Id);
        builder.Property(qm => qm.Id).HasColumnName("id");
        builder.Property(qm => qm.QuoteId).HasColumnName("quote_id");
        builder.Property(qm => qm.ModifierId).HasColumnName("modifier_id");
        builder.Property(qm => qm.AppliedValue).HasColumnName("applied_value").HasPrecision(10, 4);
        builder.Property(qm => qm.PremiumImpact).HasColumnName("premium_impact").HasPrecision(18, 2);
        builder.HasOne(qm => qm.Quote).WithMany(q => q.QuoteModifiers).HasForeignKey(qm => qm.QuoteId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(qm => qm.Modifier).WithMany(m => m.QuoteModifiers).HasForeignKey(qm => qm.ModifierId);
    }
}
