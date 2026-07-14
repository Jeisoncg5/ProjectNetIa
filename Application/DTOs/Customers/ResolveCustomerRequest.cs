namespace ProjectNetIa.Application.DTOs.Customers;

public sealed class ResolveCustomerRequest
{
    public string FullName { get; set; } = string.Empty;

    public string? DocumentNumber { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }
}
