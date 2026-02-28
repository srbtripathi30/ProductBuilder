namespace ProductBuilder.Application.DTOs.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public Guid LobId { get; set; }
    public string LobName { get; set; } = string.Empty;
    public Guid InsurerId { get; set; }
    public string InsurerName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateOnly EffectiveDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateProductRequest
{
    public Guid LobId { get; set; }
    public Guid InsurerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Version { get; set; } = "1.0";
    public DateOnly EffectiveDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
}

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateOnly EffectiveDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
}

public class UpdateProductStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
