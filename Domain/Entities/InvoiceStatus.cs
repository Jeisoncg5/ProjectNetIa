namespace ProjectNetIa.Domain.Entities;

public sealed class InvoiceStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
