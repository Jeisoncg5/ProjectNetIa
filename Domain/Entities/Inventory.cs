namespace ProjectNetIa.Domain.Entities;

public sealed class Inventory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public int MinimumQuantity { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ProductVariant? ProductVariant { get; set; }
}
