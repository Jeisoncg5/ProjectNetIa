using ProjectNetIa.Application.DTOs.Customers;

namespace ProjectNetIa.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponse> ResolveCustomerAsync(ResolveCustomerRequest request);
}
