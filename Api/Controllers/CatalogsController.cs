using Microsoft.AspNetCore.Mvc;
using ProjectNetIa.Application.Interfaces;

namespace ProjectNetIa.Api.Controllers;

[ApiController]
[Route("api/catalogs")]
public sealed class CatalogsController : ControllerBase
{
    private readonly ICatalogService _catalogService;

    public CatalogsController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet("product-categories")]
    public async Task<IActionResult> GetProductCategories()
    {
        var categories = await _catalogService.GetProductCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("sizes")]
    public async Task<IActionResult> GetSizes()
    {
        var sizes = await _catalogService.GetSizesAsync();
        return Ok(sizes);
    }

    [HttpGet("colors")]
    public async Task<IActionResult> GetColors()
    {
        var colors = await _catalogService.GetColorsAsync();
        return Ok(colors);
    }
}
