using Microsoft.EntityFrameworkCore;
using ProductBuilder.Application.DTOs.Quotes;
using ProductBuilder.Application.Interfaces;
using ProductBuilder.Infrastructure.Data;

namespace ProductBuilder.Infrastructure.Services;

public class PremiumCalculationService : IPremiumCalculationService
{
    private readonly AppDbContext _db;

    public PremiumCalculationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<QuoteDto> CalculateAsync(Guid quoteId)
    {
        var quote = await _db.Quotes
            .Include(q => q.Product)
            .Include(q => q.Broker)
            .Include(q => q.Underwriter).ThenInclude(u => u!.User)
            .Include(q => q.QuoteCovers).ThenInclude(qc => qc.Cover).ThenInclude(c => c.Premiums)
            .Include(q => q.QuoteModifiers).ThenInclude(qm => qm.Modifier)
            .FirstOrDefaultAsync(q => q.Id == quoteId)
            ?? throw new KeyNotFoundException($"Quote {quoteId} not found");

        decimal basePremium = 0;

        foreach (var qc in quote.QuoteCovers.Where(qc => qc.IsSelected))
        {
            var premium = qc.Cover.Premiums.FirstOrDefault(p => p.IsActive);
            if (premium == null) continue;

            decimal coverPremium = 0;
            switch (premium.PremiumType)
            {
                case "Flat":
                    coverPremium = premium.FlatAmount ?? 0;
                    break;
                case "RateBased":
                    coverPremium = (qc.BasisValue ?? 0) * (premium.BaseRate ?? 0);
                    break;
                case "PerUnit":
                    coverPremium = (qc.BasisValue ?? 0) * (premium.BaseRate ?? 0);
                    break;
            }

            if (premium.MinPremium.HasValue && coverPremium < premium.MinPremium.Value)
                coverPremium = premium.MinPremium.Value;

            qc.CalculatedPremium = coverPremium;
            basePremium += coverPremium;
        }

        quote.BasePremium = basePremium;
        decimal totalPremium = basePremium;

        foreach (var qm in quote.QuoteModifiers)
        {
            var modifier = qm.Modifier;
            decimal impact = 0;

            if (modifier.ValueType == "Percentage")
                impact = basePremium * (qm.AppliedValue / 100);
            else
                impact = qm.AppliedValue;

            if (modifier.ModifierType == "Discount")
                impact = -Math.Abs(impact);

            qm.PremiumImpact = impact;
            totalPremium += impact;
        }

        quote.TotalPremium = totalPremium;
        quote.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return MapToDto(quote);
    }

    private static QuoteDto MapToDto(Domain.Entities.Quote q) => new()
    {
        Id = q.Id,
        ProductId = q.ProductId,
        ProductName = q.Product.Name,
        BrokerId = q.BrokerId,
        BrokerName = q.Broker?.CompanyName,
        UnderwriterId = q.UnderwriterId,
        UnderwriterName = q.Underwriter != null ? $"{q.Underwriter.User.FirstName} {q.Underwriter.User.LastName}" : null,
        InsuredName = q.InsuredName,
        InsuredEmail = q.InsuredEmail,
        InsuredPhone = q.InsuredPhone,
        Status = q.Status,
        Currency = q.Currency,
        BasePremium = q.BasePremium,
        TotalPremium = q.TotalPremium,
        ValidUntil = q.ValidUntil,
        Notes = q.Notes,
        CreatedAt = q.CreatedAt,
        Covers = q.QuoteCovers.Select(qc => new QuoteCoverDto
        {
            Id = qc.Id,
            CoverId = qc.CoverId,
            CoverName = qc.Cover.Name,
            IsSelected = qc.IsSelected,
            SelectedLimit = qc.SelectedLimit,
            SelectedDeductible = qc.SelectedDeductible,
            CalculatedPremium = qc.CalculatedPremium,
            BasisValue = qc.BasisValue
        }).ToList(),
        Modifiers = q.QuoteModifiers.Select(qm => new QuoteModifierDto
        {
            Id = qm.Id,
            ModifierId = qm.ModifierId,
            ModifierName = qm.Modifier.Name,
            AppliedValue = qm.AppliedValue,
            PremiumImpact = qm.PremiumImpact
        }).ToList()
    };
}
