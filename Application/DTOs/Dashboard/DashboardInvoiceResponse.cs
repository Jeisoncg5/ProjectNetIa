namespace ProjectNetIa.Application.DTOs.Dashboard;

public sealed class DashboardInvoiceResponse
{
    public Guid InvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string SaleOriginName { get; set; } = string.Empty;

    public decimal Total { get; set; }
}
