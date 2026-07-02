namespace ProjectNetIa.Domain.Entities;

public sealed class Inventory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public int MinimumQuantity { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }
}
