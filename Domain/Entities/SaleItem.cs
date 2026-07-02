namespace ProjectNetIa.Domain.Entities;

public sealed class SaleItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SaleId { get; set; }

    public Guid ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public Sale? Sale { get; set; }

    public ProductVariant? ProductVariant { get; set; }
}
