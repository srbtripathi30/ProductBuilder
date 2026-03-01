using Microsoft.EntityFrameworkCore;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data.Configurations;

namespace ProductBuilder.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Underwriter> Underwriters => Set<Underwriter>();
    public DbSet<Insurer> Insurers => Set<Insurer>();
    public DbSet<Broker> Brokers => Set<Broker>();
    public DbSet<LineOfBusiness> LinesOfBusiness => Set<LineOfBusiness>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Coverage> Coverages => Set<Coverage>();
    public DbSet<Cover> Covers => Set<Cover>();
    public DbSet<Limit> Limits => Set<Limit>();
    public DbSet<Deductible> Deductibles => Set<Deductible>();
    public DbSet<Premium> Premiums => Set<Premium>();
    public DbSet<Modifier> Modifiers => Set<Modifier>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<QuoteCover> QuoteCovers => Set<QuoteCover>();
    public DbSet<QuoteModifier> QuoteModifiers => Set<QuoteModifier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Underwriter" },
            new Role { Id = 3, Name = "Broker" },
            new Role { Id = 4, Name = "Insurer" }
        );

        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminId,
            Email = "admin@productbuilder.com",
            PasswordHash = "$2a$11$ClXb35kkrElNMZw.SG3YeOw1bwXBImytjNaOek.QFmxMaK7mSnKqS", // Admin@123
            FirstName = "System",
            LastName = "Admin",
            RoleId = 1,
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<LineOfBusiness>().HasData(
            new LineOfBusiness { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Name = "Property", Code = "PROP", Description = "Property insurance", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new LineOfBusiness { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Name = "Marine", Code = "MARINE", Description = "Marine insurance", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new LineOfBusiness { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Name = "Motor", Code = "MOTOR", Description = "Motor insurance", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
