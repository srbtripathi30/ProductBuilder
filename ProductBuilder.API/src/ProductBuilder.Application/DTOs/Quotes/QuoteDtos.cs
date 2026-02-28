namespace ProductBuilder.Application.DTOs.Quotes;

public class QuoteDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid? BrokerId { get; set; }
    public string? BrokerName { get; set; }
    public Guid? UnderwriterId { get; set; }
    public string? UnderwriterName { get; set; }
    public string InsuredName { get; set; } = string.Empty;
    public string? InsuredEmail { get; set; }
    public string? InsuredPhone { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal? BasePremium { get; set; }
    public decimal? TotalPremium { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<QuoteCoverDto> Covers { get; set; } = new();
    public List<QuoteModifierDto> Modifiers { get; set; } = new();
}

public class QuoteCoverDto
{
    public Guid Id { get; set; }
    public Guid CoverId { get; set; }
    public string CoverName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
    public decimal? SelectedLimit { get; set; }
    public decimal? SelectedDeductible { get; set; }
    public decimal? CalculatedPremium { get; set; }
    public decimal? BasisValue { get; set; }
}

public class QuoteModifierDto
{
    public Guid Id { get; set; }
    public Guid ModifierId { get; set; }
    public string ModifierName { get; set; } = string.Empty;
    public decimal AppliedValue { get; set; }
    public decimal? PremiumImpact { get; set; }
}

public class CreateQuoteRequest
{
    public Guid ProductId { get; set; }
    public Guid? BrokerId { get; set; }
    public Guid? UnderwriterId { get; set; }
    public string InsuredName { get; set; } = string.Empty;
    public string? InsuredEmail { get; set; }
    public string? InsuredPhone { get; set; }
    public string Currency { get; set; } = "USD";
    public DateOnly? ValidUntil { get; set; }
    public string? Notes { get; set; }
    public List<QuoteCoverInput> Covers { get; set; } = new();
    public List<QuoteModifierInput> Modifiers { get; set; } = new();
}

public class QuoteCoverInput
{
    public Guid CoverId { get; set; }
    public bool IsSelected { get; set; } = true;
    public decimal? SelectedLimit { get; set; }
    public decimal? SelectedDeductible { get; set; }
    public decimal? BasisValue { get; set; }
}

public class QuoteModifierInput
{
    public Guid ModifierId { get; set; }
    public decimal AppliedValue { get; set; }
}
