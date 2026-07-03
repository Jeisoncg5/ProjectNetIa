using Microsoft.AspNetCore.Mvc;
using ProjectNetIa.Application.DTOs.Sales;
using ProjectNetIa.Application.Interfaces;

namespace ProjectNetIa.Api.Controllers;

[ApiController]
[Route("api/sales")]
public sealed class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSales()
    {
        var sales = await _saleService.GetSalesAsync();
        return Ok(sales);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSaleById(Guid id)
    {
        var sale = await _saleService.GetSaleByIdAsync(id);

        if (sale is null)
        {
            return NotFound(new
            {
                message = "Venta no encontrada."
            });
        }

        return Ok(sale);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale(CreateSaleRequest request)
    {
        try
        {
            var sale = await _saleService.CreateSaleAsync(request);
            return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, sale);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }
}
