namespace ProjectNetIa.Domain.Entities;

public sealed class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SaleId { get; set; }

    public int InvoiceStatusId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Sale? Sale { get; set; }

    public InvoiceStatus? InvoiceStatus { get; set; }
}
