namespace ProjectNetIa.Domain.Entities;

public sealed class CustomerDocumentType
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
