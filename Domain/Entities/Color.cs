namespace ProjectNetIa.Domain.Entities;

public sealed class Color
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? HexCode { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
