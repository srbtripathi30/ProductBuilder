namespace ProductBuilder.Domain.Entities;

public class Premium
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CoverId { get; set; }
    public string PremiumType { get; set; } = string.Empty;
    public decimal? BaseRate { get; set; }
    public decimal? FlatAmount { get; set; }
    public decimal? MinPremium { get; set; }
    public string? CalculationBasis { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsActive { get; set; } = true;

    public Cover Cover { get; set; } = null!;
}
