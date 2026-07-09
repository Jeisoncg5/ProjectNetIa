using Pgvector;

namespace ProjectNetIa.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int ProductCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public Vector? Embedding { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ProductCategory? ProductCategory { get; set; }

    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
