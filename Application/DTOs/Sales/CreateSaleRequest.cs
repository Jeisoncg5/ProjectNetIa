namespace ProjectNetIa.Application.DTOs.Sales;

public sealed class CreateSaleRequest
{
    public Guid? CustomerId { get; set; }

    public int SaleOriginId { get; set; } = 1;

    public List<CreateSaleItemRequest> Items { get; set; } = new();
}
