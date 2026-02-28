using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class CoverConfiguration : IEntityTypeConfiguration<Cover>
{
    public void Configure(EntityTypeBuilder<Cover> builder)
    {
        builder.ToTable("covers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.CoverageId).HasColumnName("coverage_id");
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(c => c.Code).HasColumnName("code").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasColumnName("description");
        builder.Property(c => c.IsMandatory).HasColumnName("is_mandatory");
        builder.Property(c => c.SequenceNo).HasColumnName("sequence_no");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.HasOne(c => c.Coverage).WithMany(cv => cv.Covers).HasForeignKey(c => c.CoverageId).OnDelete(DeleteBehavior.Cascade);
    }
}
