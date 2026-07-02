namespace ProjectNetIa.Domain.Entities;

public sealed class SaleOrigin
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
