namespace ProductBuilder.Domain.Entities;

public class Coverage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsMandatory { get; set; } = false;
    public int SequenceNo { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public ICollection<Cover> Covers { get; set; } = new List<Cover>();
}
