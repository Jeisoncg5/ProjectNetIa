using Microsoft.AspNetCore.Mvc;
using ProjectNetIa.Application.Interfaces;

namespace ProjectNetIa.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoices()
    {
        var invoices = await _invoiceService.GetInvoicesAsync();
        return Ok(invoices);
    }

    [HttpGet("by-customer/{documentNumber}")]
    public async Task<IActionResult> GetInvoicesByCustomerDocument(string documentNumber)
    {
        var invoices = await _invoiceService.GetInvoicesByCustomerDocumentAsync(documentNumber);
        return Ok(invoices);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetInvoiceById(Guid id)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

        if (invoice is null)
        {
            return NotFound(new
            {
                message = "Factura no encontrada."
            });
        }

        return Ok(invoice);
    }

    [HttpGet("number/{invoiceNumber}")]
    public async Task<IActionResult> GetInvoiceByNumber(string invoiceNumber)
    {
        var invoice = await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);

        if (invoice is null)
        {
            return NotFound(new
            {
                message = "Factura no encontrada."
            });
        }

        return Ok(invoice);
    }
}
