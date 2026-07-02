namespace ProjectNetIa.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public int? CustomerDocumentTypeId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? DocumentNumber { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CustomerDocumentType? CustomerDocumentType { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
