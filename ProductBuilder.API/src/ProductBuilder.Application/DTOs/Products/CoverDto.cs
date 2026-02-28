namespace ProductBuilder.Application.DTOs.Products;

public class CoverDto
{
    public Guid Id { get; set; }
    public Guid CoverageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; }
    public int SequenceNo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCoverRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; }
    public int SequenceNo { get; set; }
}

public class UpdateCoverRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; }
    public int SequenceNo { get; set; }
}
