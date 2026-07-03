namespace ProjectNetIa.Application.DTOs.Inventory;

public sealed class AdjustInventoryRequest
{
    public Guid ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public string? Description { get; set; }
}
