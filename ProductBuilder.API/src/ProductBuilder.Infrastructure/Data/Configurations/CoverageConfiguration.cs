using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class CoverageConfiguration : IEntityTypeConfiguration<Coverage>
{
    public void Configure(EntityTypeBuilder<Coverage> builder)
    {
        builder.ToTable("coverages");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.ProductId).HasColumnName("product_id");
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(c => c.Code).HasColumnName("code").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasColumnName("description");
        builder.Property(c => c.IsMandatory).HasColumnName("is_mandatory");
        builder.Property(c => c.SequenceNo).HasColumnName("sequence_no");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.HasOne(c => c.Product).WithMany(p => p.Coverages).HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}
