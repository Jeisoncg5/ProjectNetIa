using Microsoft.AspNetCore.Mvc;
using ProjectNetIa.Application.DTOs.Customers;
using ProjectNetIa.Application.Interfaces;

namespace ProjectNetIa.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost("resolve")]
    public async Task<IActionResult> ResolveCustomer(ResolveCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new
            {
                message = "El nombre completo del cliente es obligatorio."
            });
        }

        var customer = await _customerService.ResolveCustomerAsync(request);
        return Ok(customer);
    }
}
