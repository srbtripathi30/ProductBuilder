namespace ProductBuilder.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LobId { get; set; }
    public Guid InsurerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Version { get; set; } = "1.0";
    public string Status { get; set; } = "Draft";
    public DateOnly EffectiveDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public LineOfBusiness Lob { get; set; } = null!;
    public Insurer Insurer { get; set; } = null!;
    public User? Creator { get; set; }
    public ICollection<Coverage> Coverages { get; set; } = new List<Coverage>();
    public ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
