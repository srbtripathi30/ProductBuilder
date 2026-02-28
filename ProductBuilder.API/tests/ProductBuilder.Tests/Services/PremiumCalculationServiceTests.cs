using Microsoft.EntityFrameworkCore;
using ProductBuilder.Domain.Entities;
using ProductBuilder.Infrastructure.Data;
using ProductBuilder.Infrastructure.Services;

namespace ProductBuilder.Tests.Services;

public class PremiumCalculationServiceTests
{
    // Creates a fresh isolated in-memory DB per test
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    /// <summary>
    /// Seeds a minimal quote with one cover and one active premium, returns the quoteId.
    /// BasisValue defaults to 100_000 for rate-based tests.
    /// </summary>
    private static Guid SeedQuote(
        AppDbContext db,
        string premiumType = "Flat",
        decimal flatAmount = 1000m,
        decimal baseRate = 0m,
        decimal? basisValue = null,
        decimal? minPremium = null,
        bool isSelected = true)
    {
        var productId = Guid.NewGuid();
        var coverageId = Guid.NewGuid();
        var coverId = Guid.NewGuid();
        var quoteId = Guid.NewGuid();

        db.Products.Add(new Product
        {
            Id = productId,
            LobId = Guid.NewGuid(),      // FK not enforced by InMemory
            InsurerId = Guid.NewGuid(),
            Name = "Test Product",
            Code = $"PRD-{productId:N}",
            Version = "1.0",
            Status = "Active",
            EffectiveDate = DateOnly.FromDateTime(DateTime.Today),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        db.Coverages.Add(new Coverage
        {
            Id = coverageId, ProductId = productId,
            Name = "Fire", Code = "FIRE", CreatedAt = DateTime.UtcNow
        });

        db.Covers.Add(new Cover
        {
            Id = coverId, CoverageId = coverageId,
            Name = "Building", Code = "BLDG", CreatedAt = DateTime.UtcNow
        });

        db.Premiums.Add(new Premium
        {
            Id = Guid.NewGuid(), CoverId = coverId,
            PremiumType = premiumType,
            FlatAmount = premiumType == "Flat" ? flatAmount : null,
            BaseRate = premiumType != "Flat" ? baseRate : null,
            MinPremium = minPremium,
            Currency = "USD",
            IsActive = true
        });

        db.Quotes.Add(new Quote
        {
            Id = quoteId, ProductId = productId,
            InsuredName = "Acme Corp", Status = "Draft", Currency = "USD",
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        });

        db.QuoteCovers.Add(new QuoteCover
        {
            Id = Guid.NewGuid(), QuoteId = quoteId, CoverId = coverId,
            IsSelected = isSelected,
            BasisValue = basisValue ?? 100_000m
        });

        db.SaveChanges();
        return quoteId;
    }

    private static Guid AddModifier(
        AppDbContext db, Guid quoteId,
        string modifierType, string valueType, decimal appliedValue)
    {
        var modId = Guid.NewGuid();
        db.Modifiers.Add(new Modifier
        {
            Id = modId, Name = "Test Modifier", Code = $"MOD-{modId:N}",
            ModifierType = modifierType, ValueType = valueType,
            MinValue = 0, MaxValue = 100, IsActive = true
        });
        db.QuoteModifiers.Add(new QuoteModifier
        {
            Id = Guid.NewGuid(), QuoteId = quoteId, ModifierId = modId,
            AppliedValue = appliedValue
        });
        db.SaveChanges();
        return modId;
    }

    // ── Flat Premium ─────────────────────────────────────────────────────────

    [Fact]
    public async Task CalculateAsync_FlatPremium_SetsBasePremium()
    {
        using var db = CreateDb();
        var quoteId = SeedQuote(db, "Flat", flatAmount: 1500m);
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(1500m, result.BasePremium);
        Assert.Equal(1500m, result.TotalPremium);
    }

    [Fact]
    public async Task CalculateAsync_FlatPremium_ReturnsCorrectCoverPremium()
    {
        using var db = CreateDb();
        var quoteId = SeedQuote(db, "Flat", flatAmount: 800m);
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Single(result.Covers);
        Assert.Equal(800m, result.Covers[0].CalculatedPremium);
    }

    // ── Rate-Based Premium ────────────────────────────────────────────────────

    [Fact]
    public async Task CalculateAsync_RateBasedPremium_CalculatesRateTimesBasisValue()
    {
        using var db = CreateDb();
        // rate=0.01, basisValue=500_000 → expected=5000
        var quoteId = SeedQuote(db, "RateBased", baseRate: 0.01m, basisValue: 500_000m);
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(5000m, result.BasePremium);
    }

    [Fact]
    public async Task CalculateAsync_PerUnitPremium_CalculatesRateTimesBasisValue()
    {
        using var db = CreateDb();
        // rate=0.005, basisValue=200_000 → expected=1000
        var quoteId = SeedQuote(db, "PerUnit", baseRate: 0.005m, basisValue: 200_000m);
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(1000m, result.BasePremium);
    }

    // ── Min Premium Enforcement ───────────────────────────────────────────────

    [Fact]
    public async Task CalculateAsync_PremiumBelowMinimum_UsesMinPremium()
    {
        using var db = CreateDb();
        // rate=0.001, basis=1000 → calculated=1, minPremium=250 → should use 250
        var quoteId = SeedQuote(db, "RateBased", baseRate: 0.001m, basisValue: 1000m, minPremium: 250m);
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(250m, result.BasePremium);
    }

    [Fact]
    public async Task CalculateAsync_PremiumAboveMinimum_UsesCalculatedPremium()
    {
        using var db = CreateDb();
        // flat=2000, minPremium=500 → should use 2000 (already above min)
        var quoteId = SeedQuote(db, "Flat", flatAmount: 2000m, minPremium: 500m);
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(2000m, result.BasePremium);
    }

    // ── Modifiers ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CalculateAsync_PercentageLoadingModifier_IncreasesTotalPremium()
    {
        using var db = CreateDb();
        var quoteId = SeedQuote(db, "Flat", flatAmount: 1000m);
        AddModifier(db, quoteId, "Loading", "Percentage", 10m); // +10%
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(1000m, result.BasePremium);
        Assert.Equal(1100m, result.TotalPremium); // 1000 + 100
    }

    [Fact]
    public async Task CalculateAsync_PercentageDiscountModifier_DecreasesTotalPremium()
    {
        using var db = CreateDb();
        var quoteId = SeedQuote(db, "Flat", flatAmount: 1000m);
        AddModifier(db, quoteId, "Discount", "Percentage", 20m); // -20%
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(1000m, result.BasePremium);
        Assert.Equal(800m, result.TotalPremium); // 1000 - 200
    }

    [Fact]
    public async Task CalculateAsync_FixedLoadingModifier_AddsExactAmount()
    {
        using var db = CreateDb();
        var quoteId = SeedQuote(db, "Flat", flatAmount: 1000m);
        AddModifier(db, quoteId, "Loading", "Fixed", 150m); // +$150
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(1150m, result.TotalPremium);
    }

    [Fact]
    public async Task CalculateAsync_FixedDiscountModifier_SubtractsExactAmount()
    {
        using var db = CreateDb();
        var quoteId = SeedQuote(db, "Flat", flatAmount: 1000m);
        AddModifier(db, quoteId, "Discount", "Fixed", 100m); // -$100
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(900m, result.TotalPremium);
    }

    [Fact]
    public async Task CalculateAsync_MultipleModifiers_AllApplied()
    {
        using var db = CreateDb();
        var quoteId = SeedQuote(db, "Flat", flatAmount: 1000m);
        AddModifier(db, quoteId, "Loading", "Percentage", 10m);  // +100
        AddModifier(db, quoteId, "Discount", "Percentage", 5m);   // -50
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        // base=1000, +10%=+100, -5%=-50 → total=1050
        Assert.Equal(1050m, result.TotalPremium);
    }

    // ── Edge Cases ────────────────────────────────────────────────────────────

    [Fact]
    public async Task CalculateAsync_UnselectedCover_ExcludedFromCalculation()
    {
        using var db = CreateDb();
        var quoteId = SeedQuote(db, "Flat", flatAmount: 1000m, isSelected: false);
        var result = await new PremiumCalculationService(db).CalculateAsync(quoteId);

        Assert.Equal(0m, result.BasePremium);
        Assert.Equal(0m, result.TotalPremium);
    }

    [Fact]
    public async Task CalculateAsync_QuoteNotFound_ThrowsKeyNotFoundException()
    {
        using var db = CreateDb();
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            new PremiumCalculationService(db).CalculateAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CalculateAsync_ModifierImpactStoredOnQuoteModifier()
    {
        using var db = CreateDb();
        var quoteId = SeedQuote(db, "Flat", flatAmount: 1000m);
        AddModifier(db, quoteId, "Loading", "Percentage", 15m);
        await new PremiumCalculationService(db).CalculateAsync(quoteId);

        var qm = db.QuoteModifiers.First(m => m.QuoteId == quoteId);
        Assert.Equal(150m, qm.PremiumImpact); // 15% of 1000
    }
}
