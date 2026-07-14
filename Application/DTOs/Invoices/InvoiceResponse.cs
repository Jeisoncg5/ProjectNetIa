namespace ProjectNetIa.Application.DTOs.Invoices;

public sealed class InvoiceResponse
{
    public Guid Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public Guid SaleId { get; set; }

    public string InvoiceStatusName { get; set; } = string.Empty;

    public string SaleOriginName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public decimal Total { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerDocument { get; set; }

    public List<InvoiceItemResponse> Items { get; set; } = new();
}
