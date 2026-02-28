namespace ProductBuilder.Domain.Entities;

public class Modifier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? CoverId { get; set; }
    public Guid? ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string ModifierType { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public decimal MinValue { get; set; } = 0;
    public decimal MaxValue { get; set; }
    public decimal? DefaultValue { get; set; }
    public bool IsMandatory { get; set; } = false;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Cover? Cover { get; set; }
    public Product? Product { get; set; }
    public ICollection<QuoteModifier> QuoteModifiers { get; set; } = new List<QuoteModifier>();
}
