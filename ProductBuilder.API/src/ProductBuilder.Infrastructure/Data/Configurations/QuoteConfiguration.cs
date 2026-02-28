using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.ToTable("quotes");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).HasColumnName("id");
        builder.Property(q => q.ProductId).HasColumnName("product_id");
        builder.Property(q => q.BrokerId).HasColumnName("broker_id");
        builder.Property(q => q.UnderwriterId).HasColumnName("underwriter_id");
        builder.Property(q => q.InsuredName).HasColumnName("insured_name").HasMaxLength(255).IsRequired();
        builder.Property(q => q.InsuredEmail).HasColumnName("insured_email").HasMaxLength(255);
        builder.Property(q => q.InsuredPhone).HasColumnName("insured_phone").HasMaxLength(50);
        builder.Property(q => q.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(q => q.Currency).HasColumnName("currency").HasMaxLength(10).IsRequired();
        builder.Property(q => q.BasePremium).HasColumnName("base_premium").HasPrecision(18, 2);
        builder.Property(q => q.TotalPremium).HasColumnName("total_premium").HasPrecision(18, 2);
        builder.Property(q => q.ValidUntil).HasColumnName("valid_until");
        builder.Property(q => q.Notes).HasColumnName("notes");
        builder.Property(q => q.CreatedBy).HasColumnName("created_by");
        builder.Property(q => q.CreatedAt).HasColumnName("created_at");
        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at");
        builder.HasOne(q => q.Product).WithMany(p => p.Quotes).HasForeignKey(q => q.ProductId);
        builder.HasOne(q => q.Broker).WithMany(b => b.Quotes).HasForeignKey(q => q.BrokerId);
        builder.HasOne(q => q.Underwriter).WithMany(u => u.Quotes).HasForeignKey(q => q.UnderwriterId);
        builder.HasOne(q => q.Creator).WithMany().HasForeignKey(q => q.CreatedBy);
    }
}
