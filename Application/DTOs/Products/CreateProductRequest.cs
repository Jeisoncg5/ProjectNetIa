namespace ProjectNetIa.Application.DTOs.Products;

public sealed class CreateProductRequest
{
    public int ProductCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }
}
