namespace ProductBuilder.Domain.Entities;

public class Quote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public Guid? BrokerId { get; set; }
    public Guid? UnderwriterId { get; set; }
    public string InsuredName { get; set; } = string.Empty;
    public string? InsuredEmail { get; set; }
    public string? InsuredPhone { get; set; }
    public string Status { get; set; } = "Draft";
    public string Currency { get; set; } = "USD";
    public decimal? BasePremium { get; set; }
    public decimal? TotalPremium { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public string? Notes { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public Broker? Broker { get; set; }
    public Underwriter? Underwriter { get; set; }
    public User? Creator { get; set; }
    public ICollection<QuoteCover> QuoteCovers { get; set; } = new List<QuoteCover>();
    public ICollection<QuoteModifier> QuoteModifiers { get; set; } = new List<QuoteModifier>();
}
