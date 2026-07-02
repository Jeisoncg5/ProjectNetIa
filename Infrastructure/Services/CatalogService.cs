using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Application.DTOs.Catalogs;
using ProjectNetIa.Application.Interfaces;
using ProjectNetIa.Infrastructure.Data;

namespace ProjectNetIa.Infrastructure.Services;

public sealed class CatalogService : ICatalogService
{
    private readonly ApplicationDbContext _context;

    public CatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CatalogResponse>> GetProductCategoriesAsync()
    {
        return await _context.ProductCategories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.Name)
            .Select(category => new CatalogResponse
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CatalogResponse>> GetSizesAsync()
    {
        return await _context.Sizes
            .AsNoTracking()
            .Where(size => size.IsActive)
            .OrderBy(size => size.Name)
            .Select(size => new CatalogResponse
            {
                Id = size.Id,
                Name = size.Name
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CatalogResponse>> GetColorsAsync()
    {
        return await _context.Colors
            .AsNoTracking()
            .Where(color => color.IsActive)
            .OrderBy(color => color.Name)
            .Select(color => new CatalogResponse
            {
                Id = color.Id,
                Name = color.Name
            })
            .ToListAsync();
    }
}
