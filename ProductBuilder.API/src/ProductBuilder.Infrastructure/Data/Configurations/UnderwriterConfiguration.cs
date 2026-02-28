using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class UnderwriterConfiguration : IEntityTypeConfiguration<Underwriter>
{
    public void Configure(EntityTypeBuilder<Underwriter> builder)
    {
        builder.ToTable("underwriters");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");
        builder.Property(u => u.UserId).HasColumnName("user_id");
        builder.Property(u => u.LicenseNo).HasColumnName("license_no").HasMaxLength(100);
        builder.Property(u => u.Specialization).HasColumnName("specialization").HasMaxLength(255);
        builder.Property(u => u.AuthorityLimit).HasColumnName("authority_limit").HasPrecision(18, 2);
        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
        builder.HasOne(u => u.User).WithOne(u => u.Underwriter).HasForeignKey<Underwriter>(u => u.UserId);
    }
}
