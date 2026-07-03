namespace ProjectNetIa.Application.DTOs.Sales;

public sealed class CreateSaleItemRequest
{
    public Guid ProductVariantId { get; set; }

    public int Quantity { get; set; }
}
