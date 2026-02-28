namespace ProductBuilder.Domain.Entities;

public class Broker
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid? InsurerId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? LicenseNo { get; set; }
    public decimal? CommissionRate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Insurer? Insurer { get; set; }
    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
