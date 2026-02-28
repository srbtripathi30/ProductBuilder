namespace ProductBuilder.Domain.Entities;

public class Cover
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CoverageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; } = false;
    public int SequenceNo { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Coverage Coverage { get; set; } = null!;
    public ICollection<Limit> Limits { get; set; } = new List<Limit>();
    public ICollection<Deductible> Deductibles { get; set; } = new List<Deductible>();
    public ICollection<Premium> Premiums { get; set; } = new List<Premium>();
    public ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
    public ICollection<QuoteCover> QuoteCovers { get; set; } = new List<QuoteCover>();
}
