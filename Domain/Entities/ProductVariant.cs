namespace ProjectNetIa.Domain.Entities;

public sealed class ProductVariant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ProductId { get; set; }

    public int SizeId { get; set; }

    public int ColorId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public Product? Product { get; set; }

    public Size? Size { get; set; }

    public Color? Color { get; set; }

    public Inventory? Inventory { get; set; }

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}
