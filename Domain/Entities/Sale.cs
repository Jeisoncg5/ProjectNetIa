namespace ProjectNetIa.Domain.Entities;

public sealed class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? CustomerId { get; set; }

    public int SaleOriginId { get; set; }

    public int SaleStatusId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }

    public SaleOrigin? SaleOrigin { get; set; }

    public SaleStatus? SaleStatus { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    public Invoice? Invoice { get; set; }
}
