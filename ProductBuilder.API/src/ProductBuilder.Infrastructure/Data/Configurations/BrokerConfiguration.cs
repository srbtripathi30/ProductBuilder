using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class BrokerConfiguration : IEntityTypeConfiguration<Broker>
{
    public void Configure(EntityTypeBuilder<Broker> builder)
    {
        builder.ToTable("brokers");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasColumnName("id");
        builder.Property(b => b.UserId).HasColumnName("user_id");
        builder.Property(b => b.InsurerId).HasColumnName("insurer_id");
        builder.Property(b => b.CompanyName).HasColumnName("company_name").HasMaxLength(255).IsRequired();
        builder.Property(b => b.LicenseNo).HasColumnName("license_no").HasMaxLength(100);
        builder.Property(b => b.CommissionRate).HasColumnName("commission_rate").HasPrecision(5, 2);
        builder.Property(b => b.IsActive).HasColumnName("is_active");
        builder.Property(b => b.CreatedAt).HasColumnName("created_at");
        builder.HasOne(b => b.User).WithOne(u => u.Broker).HasForeignKey<Broker>(b => b.UserId);
        builder.HasOne(b => b.Insurer).WithMany(i => i.Brokers).HasForeignKey(b => b.InsurerId);
    }
}
