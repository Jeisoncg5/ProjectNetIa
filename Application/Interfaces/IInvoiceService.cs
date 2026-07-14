using ProjectNetIa.Application.DTOs.Invoices;

namespace ProjectNetIa.Application.Interfaces;

public interface IInvoiceService
{
    Task<IReadOnlyList<InvoiceResponse>> GetInvoicesAsync();

    Task<InvoiceResponse?> GetInvoiceByIdAsync(Guid id);

    Task<InvoiceResponse?> GetInvoiceByNumberAsync(string invoiceNumber);

    Task<IReadOnlyList<InvoiceResponse>> GetInvoicesByCustomerDocumentAsync(string documentNumber);
}
