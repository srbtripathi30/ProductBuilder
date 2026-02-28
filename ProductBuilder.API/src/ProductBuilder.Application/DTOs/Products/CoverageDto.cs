namespace ProductBuilder.Application.DTOs.Products;

public class CoverageDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; }
    public int SequenceNo { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CoverDto> Covers { get; set; } = new();
}

public class CreateCoverageRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; }
    public int SequenceNo { get; set; }
}

public class UpdateCoverageRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; }
    public int SequenceNo { get; set; }
}
