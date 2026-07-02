namespace ProjectNetIa.Domain.Entities;

public sealed class InventoryMovement
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public int InventoryMovementTypeId { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }

    public InventoryMovementType? InventoryMovementType { get; set; }
}
