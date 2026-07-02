namespace ProjectNetIa.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int ProductCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ProductCategory? ProductCategory { get; set; }

    public Inventory? Inventory { get; set; }

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}
