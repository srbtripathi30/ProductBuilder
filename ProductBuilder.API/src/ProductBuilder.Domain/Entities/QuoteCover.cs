namespace ProductBuilder.Domain.Entities;

public class QuoteCover
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid QuoteId { get; set; }
    public Guid CoverId { get; set; }
    public bool IsSelected { get; set; } = true;
    public decimal? SelectedLimit { get; set; }
    public decimal? SelectedDeductible { get; set; }
    public decimal? CalculatedPremium { get; set; }
    public decimal? BasisValue { get; set; }

    public Quote Quote { get; set; } = null!;
    public Cover Cover { get; set; } = null!;
}
