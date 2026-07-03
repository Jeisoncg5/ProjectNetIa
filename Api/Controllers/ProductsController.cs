using Microsoft.AspNetCore.Mvc;
using ProjectNetIa.Application.DTOs.Products;
using ProjectNetIa.Application.Interfaces;

namespace ProjectNetIa.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetProductsAsync();
        return Ok(products);
    }

    [HttpGet("variants/search")]
    public async Task<IActionResult> SearchProductVariants(
        [FromQuery] string? query,
        [FromQuery] string? size,
        [FromQuery] string? color,
        [FromQuery] bool onlyAvailable = true)
    {
        var variants = await _productService.SearchProductVariantsAsync(
            query,
            size,
            color,
            onlyAvailable
        );

        return Ok(variants);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product is null)
        {
            return NotFound(new
            {
                message = "Producto no encontrado."
            });
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductRequest request)
    {
        try
        {
            var product = await _productService.CreateProductAsync(request);

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product
            );
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPost("variants")]
    public async Task<IActionResult> CreateProductVariant(CreateProductVariantRequest request)
    {
        try
        {
            var variant = await _productService.CreateProductVariantAsync(request);
            return Ok(variant);
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
