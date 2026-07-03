using ProjectNetIa.Application.DTOs.Sales;

namespace ProjectNetIa.Application.Interfaces;

public interface ISaleService
{
    Task<SaleResponse> CreateSaleAsync(CreateSaleRequest request);

    Task<IReadOnlyList<SaleResponse>> GetSalesAsync();

    Task<SaleResponse?> GetSaleByIdAsync(Guid id);
}
