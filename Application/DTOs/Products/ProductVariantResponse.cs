namespace ProjectNetIa.Application.DTOs.Products;

public sealed class ProductVariantResponse
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int SizeId { get; set; }

    public string SizeName { get; set; } = string.Empty;

    public int ColorId { get; set; }

    public string ColorName { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int Quantity { get; set; }

    public int MinimumQuantity { get; set; }
}
