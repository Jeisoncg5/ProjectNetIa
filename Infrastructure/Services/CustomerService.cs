using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Application.DTOs.Customers;
using ProjectNetIa.Application.Interfaces;
using ProjectNetIa.Domain.Entities;
using ProjectNetIa.Infrastructure.Data;

namespace ProjectNetIa.Infrastructure.Services;

public sealed class CustomerService : ICustomerService
{
    private const int DefaultCustomerDocumentTypeId = 1;

    private readonly ApplicationDbContext _context;

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerResponse> ResolveCustomerAsync(ResolveCustomerRequest request)
    {
        var fullName = request.FullName.Trim();
        var documentNumber = string.IsNullOrWhiteSpace(request.DocumentNumber)
            ? null
            : request.DocumentNumber.Trim();
        var email = string.IsNullOrWhiteSpace(request.Email)
            ? null
            : request.Email.Trim();
        var phone = string.IsNullOrWhiteSpace(request.Phone)
            ? null
            : request.Phone.Trim();

        if (documentNumber is not null)
        {
            var normalizedDocument = documentNumber.ToLowerInvariant();
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(customer =>
                    customer.DocumentNumber != null
                    && customer.DocumentNumber.ToLower() == normalizedDocument);

            if (existingCustomer is not null)
            {
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    existingCustomer.FullName = fullName;
                }

                if (email is not null)
                {
                    existingCustomer.Email = email;
                }

                if (phone is not null)
                {
                    existingCustomer.Phone = phone;
                }

                await _context.SaveChangesAsync();
                return MapToResponse(existingCustomer);
            }
        }

        var customer = new Customer
        {
            FullName = fullName,
            DocumentNumber = documentNumber,
            Email = email,
            Phone = phone,
            CustomerDocumentTypeId = DefaultCustomerDocumentTypeId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return MapToResponse(customer);
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            FullName = customer.FullName,
            DocumentNumber = customer.DocumentNumber,
            Email = customer.Email,
            Phone = customer.Phone
        };
    }
}
