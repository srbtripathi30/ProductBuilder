namespace ProductBuilder.Domain.Entities;

public class Underwriter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string? LicenseNo { get; set; }
    public string? Specialization { get; set; }
    public decimal? AuthorityLimit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
