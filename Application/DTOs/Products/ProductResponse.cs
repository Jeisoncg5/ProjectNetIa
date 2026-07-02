namespace ProjectNetIa.Application.DTOs.Products;

public sealed class ProductResponse
{
    public Guid Id { get; set; }

    public int ProductCategoryId { get; set; }

    public string ProductCategoryName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public List<ProductVariantResponse> Variants { get; set; } = new();
}
