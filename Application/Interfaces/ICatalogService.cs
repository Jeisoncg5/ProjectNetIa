using ProjectNetIa.Application.DTOs.Catalogs;

namespace ProjectNetIa.Application.Interfaces;

public interface ICatalogService
{
    Task<IReadOnlyList<CatalogResponse>> GetProductCategoriesAsync();

    Task<IReadOnlyList<CatalogResponse>> GetSizesAsync();

    Task<IReadOnlyList<CatalogResponse>> GetColorsAsync();
}
