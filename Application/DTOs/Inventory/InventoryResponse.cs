namespace ProjectNetIa.Application.DTOs.Inventory;

public sealed class InventoryResponse
{
    public Guid InventoryId { get; set; }

    public Guid ProductVariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public string SizeName { get; set; } = string.Empty;

    public string ColorName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public int MinimumQuantity { get; set; }

    public bool IsLowStock { get; set; }
}
