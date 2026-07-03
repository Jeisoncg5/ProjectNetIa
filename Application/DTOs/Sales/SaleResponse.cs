namespace ProjectNetIa.Application.DTOs.Sales;

public sealed class SaleResponse
{
    public Guid Id { get; set; }

    public Guid? CustomerId { get; set; }

    public string SaleOriginName { get; set; } = string.Empty;

    public string SaleStatusName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public List<SaleItemResponse> Items { get; set; } = new();
}
