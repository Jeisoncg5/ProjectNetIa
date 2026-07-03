namespace ProjectNetIa.Application.DTOs.Sales;

public sealed class SaleItemResponse
{
    public Guid ProductVariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public string SizeName { get; set; } = string.Empty;

    public string ColorName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Subtotal { get; set; }
}
