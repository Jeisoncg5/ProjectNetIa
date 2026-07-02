namespace ProjectNetIa.Application.DTOs.Products;

public sealed class CreateProductVariantRequest
{
    public Guid ProductId { get; set; }

    public int SizeId { get; set; }

    public int ColorId { get; set; }

    public string Sku { get; set; } = string.Empty;

    public int InitialQuantity { get; set; }

    public int MinimumQuantity { get; set; }
}
