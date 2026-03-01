using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductBuilder.Domain.Entities;

namespace ProductBuilder.Infrastructure.Data.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.UserId).HasColumnName("user_id");
        builder.Property(p => p.Token).HasColumnName("token").HasMaxLength(128).IsRequired();
        builder.Property(p => p.ExpiresAt).HasColumnName("expires_at");
        builder.Property(p => p.UsedAt).HasColumnName("used_at");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.HasIndex(p => p.Token).IsUnique();
        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.Ignore(p => p.IsExpired);
        builder.Ignore(p => p.IsUsed);
        builder.Ignore(p => p.IsValid);
    }
}
