using Microsoft.AspNetCore.Mvc;
using ProjectNetIa.Application.DTOs.Inventory;
using ProjectNetIa.Application.Interfaces;

namespace ProjectNetIa.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetInventory()
    {
        var inventory = await _inventoryService.GetInventoryAsync();
        return Ok(inventory);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var inventory = await _inventoryService.GetLowStockAsync();
        return Ok(inventory);
    }

    [HttpGet("variant/{productVariantId:guid}")]
    public async Task<IActionResult> GetInventoryByVariantId(Guid productVariantId)
    {
        var inventory = await _inventoryService.GetInventoryByVariantIdAsync(productVariantId);

        if (inventory is null)
        {
            return NotFound(new
            {
                message = "Inventario no encontrado para la variante indicada."
            });
        }

        return Ok(inventory);
    }

    [HttpPost("adjust")]
    public async Task<IActionResult> AdjustInventory(AdjustInventoryRequest request)
    {
        try
        {
            var inventory = await _inventoryService.AdjustInventoryAsync(request);
            return Ok(inventory);
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
