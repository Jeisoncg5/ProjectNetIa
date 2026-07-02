using ProjectNetIa.Application.DTOs.Products;

namespace ProjectNetIa.Application.Interfaces;

public interface IProductService
{
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);

    Task<ProductVariantResponse> CreateProductVariantAsync(CreateProductVariantRequest request);

    Task<IReadOnlyList<ProductResponse>> GetProductsAsync();

    Task<ProductResponse?> GetProductByIdAsync(Guid id);
}
