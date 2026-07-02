namespace ProjectNetIa.Domain.Entities;

public sealed class InventoryMovementType
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}
