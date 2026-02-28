namespace ProductBuilder.Application.DTOs.Financials;

public class LimitDto
{
    public Guid Id { get; set; }
    public Guid CoverId { get; set; }
    public string LimitType { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal DefaultAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateLimitRequest
{
    public string LimitType { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal DefaultAmount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class UpdateLimitRequest : CreateLimitRequest
{
    public bool IsActive { get; set; }
}

public class DeductibleDto
{
    public Guid Id { get; set; }
    public Guid CoverId { get; set; }
    public string DeductibleType { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal DefaultAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateDeductibleRequest
{
    public string DeductibleType { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal DefaultAmount { get; set; }
    public string Currency { get; set; } = "USD";
}

public class UpdateDeductibleRequest : CreateDeductibleRequest
{
    public bool IsActive { get; set; }
}

public class PremiumDto
{
    public Guid Id { get; set; }
    public Guid CoverId { get; set; }
    public string PremiumType { get; set; } = string.Empty;
    public decimal? BaseRate { get; set; }
    public decimal? FlatAmount { get; set; }
    public decimal? MinPremium { get; set; }
    public string? CalculationBasis { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreatePremiumRequest
{
    public string PremiumType { get; set; } = string.Empty;
    public decimal? BaseRate { get; set; }
    public decimal? FlatAmount { get; set; }
    public decimal? MinPremium { get; set; }
    public string? CalculationBasis { get; set; }
    public string Currency { get; set; } = "USD";
}

public class UpdatePremiumRequest : CreatePremiumRequest
{
    public bool IsActive { get; set; }
}

public class ModifierDto
{
    public Guid Id { get; set; }
    public Guid? CoverId { get; set; }
    public Guid? ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string ModifierType { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public decimal MinValue { get; set; }
    public decimal MaxValue { get; set; }
    public decimal? DefaultValue { get; set; }
    public bool IsMandatory { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class CreateModifierRequest
{
    public Guid? CoverId { get; set; }
    public Guid? ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string ModifierType { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public decimal MinValue { get; set; }
    public decimal MaxValue { get; set; }
    public decimal? DefaultValue { get; set; }
    public bool IsMandatory { get; set; }
    public string? Description { get; set; }
}

public class UpdateModifierRequest : CreateModifierRequest
{
    public bool IsActive { get; set; }
}
