namespace ProductBuilder.Domain.Entities;

public class QuoteModifier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid QuoteId { get; set; }
    public Guid ModifierId { get; set; }
    public decimal AppliedValue { get; set; }
    public decimal? PremiumImpact { get; set; }

    public Quote Quote { get; set; } = null!;
    public Modifier Modifier { get; set; } = null!;
}
