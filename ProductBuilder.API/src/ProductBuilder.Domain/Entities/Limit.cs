namespace ProductBuilder.Domain.Entities;

public class Limit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CoverId { get; set; }
    public string LimitType { get; set; } = string.Empty;
    public decimal MinAmount { get; set; } = 0;
    public decimal MaxAmount { get; set; }
    public decimal DefaultAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public bool IsActive { get; set; } = true;

    public Cover Cover { get; set; } = null!;
}
