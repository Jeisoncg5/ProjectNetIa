using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Application.DTOs.Products;
using ProjectNetIa.Application.Interfaces;
using ProjectNetIa.Domain.Entities;
using ProjectNetIa.Infrastructure.Data;

namespace ProjectNetIa.Infrastructure.Services;

public sealed class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("El nombre del producto es obligatorio.");
        }

        if (request.Price <= 0)
        {
            throw new InvalidOperationException("El precio del producto debe ser mayor que cero.");
        }

        var categoryExists = await _context.ProductCategories
            .AnyAsync(category =>
                category.Id == request.ProductCategoryId &&
                category.IsActive);

        if (!categoryExists)
        {
            throw new InvalidOperationException("La categoria del producto no existe o esta inactiva.");
        }

        var product = new Product
        {
            ProductCategoryId = request.ProductCategoryId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var createdProduct = await GetProductByIdAsync(product.Id);

        if (createdProduct is null)
        {
            throw new InvalidOperationException("El producto fue creado, pero no pudo ser consultado.");
        }

        return createdProduct;
    }

    public async Task<ProductVariantResponse> CreateProductVariantAsync(CreateProductVariantRequest request)
    {
        if (request.InitialQuantity < 0)
        {
            throw new InvalidOperationException("La cantidad inicial no puede ser negativa.");
        }

        if (request.MinimumQuantity < 0)
        {
            throw new InvalidOperationException("La cantidad minima no puede ser negativa.");
        }

        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            throw new InvalidOperationException("El SKU es obligatorio.");
        }

        var productExists = await _context.Products
            .AnyAsync(product =>
                product.Id == request.ProductId &&
                product.IsActive);

        if (!productExists)
        {
            throw new InvalidOperationException("El producto no existe o esta inactivo.");
        }

        var sizeExists = await _context.Sizes
            .AnyAsync(size =>
                size.Id == request.SizeId &&
                size.IsActive);

        if (!sizeExists)
        {
            throw new InvalidOperationException("La talla no existe o esta inactiva.");
        }

        var colorExists = await _context.Colors
            .AnyAsync(color =>
                color.Id == request.ColorId &&
                color.IsActive);

        if (!colorExists)
        {
            throw new InvalidOperationException("El color no existe o esta inactivo.");
        }

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();

        var skuExists = await _context.ProductVariants
            .AnyAsync(variant => variant.Sku == normalizedSku);

        if (skuExists)
        {
            throw new InvalidOperationException("Ya existe una variante con ese SKU.");
        }

        var variantExists = await _context.ProductVariants
            .AnyAsync(variant =>
                variant.ProductId == request.ProductId &&
                variant.SizeId == request.SizeId &&
                variant.ColorId == request.ColorId);

        if (variantExists)
        {
            throw new InvalidOperationException("Ya existe una variante para ese producto, talla y color.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var productVariant = new ProductVariant
        {
            ProductId = request.ProductId,
            SizeId = request.SizeId,
            ColorId = request.ColorId,
            Sku = normalizedSku,
            IsActive = true
        };

        _context.ProductVariants.Add(productVariant);

        var inventory = new Inventory
        {
            ProductVariantId = productVariant.Id,
            Quantity = request.InitialQuantity,
            MinimumQuantity = request.MinimumQuantity,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Inventories.Add(inventory);

        if (request.InitialQuantity > 0)
        {
            const int initialStockMovementTypeId = 1;

            var movement = new InventoryMovement
            {
                ProductVariantId = productVariant.Id,
                InventoryMovementTypeId = initialStockMovementTypeId,
                Quantity = request.InitialQuantity,
                Description = "Inventario inicial de la variante.",
                CreatedAt = DateTime.UtcNow
            };

            _context.InventoryMovements.Add(movement);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var createdVariant = await GetProductVariantByIdAsync(productVariant.Id);

        if (createdVariant is null)
        {
            throw new InvalidOperationException("La variante fue creada, pero no pudo ser consultada.");
        }

        return createdVariant;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetProductsAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Include(product => product.ProductCategory)
            .Include(product => product.ProductVariants)
                .ThenInclude(variant => variant.Size)
            .Include(product => product.ProductVariants)
                .ThenInclude(variant => variant.Color)
            .Include(product => product.ProductVariants)
                .ThenInclude(variant => variant.Inventory)
            .OrderBy(product => product.Name)
            .Select(product => new ProductResponse
            {
                Id = product.Id,
                ProductCategoryId = product.ProductCategoryId,
                ProductCategoryName = product.ProductCategory != null
                    ? product.ProductCategory.Name
                    : string.Empty,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsActive = product.IsActive,
                Variants = product.ProductVariants
                    .OrderBy(variant => variant.Sku)
                    .Select(variant => new ProductVariantResponse
                    {
                        Id = variant.Id,
                        ProductId = variant.ProductId,
                        ProductName = product.Name,
                        SizeId = variant.SizeId,
                        SizeName = variant.Size != null ? variant.Size.Name : string.Empty,
                        ColorId = variant.ColorId,
                        ColorName = variant.Color != null ? variant.Color.Name : string.Empty,
                        Sku = variant.Sku,
                        IsActive = variant.IsActive,
                        Quantity = variant.Inventory != null ? variant.Inventory.Quantity : 0,
                        MinimumQuantity = variant.Inventory != null ? variant.Inventory.MinimumQuantity : 0
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<ProductResponse?> GetProductByIdAsync(Guid id)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(product => product.ProductCategory)
            .Include(product => product.ProductVariants)
                .ThenInclude(variant => variant.Size)
            .Include(product => product.ProductVariants)
                .ThenInclude(variant => variant.Color)
            .Include(product => product.ProductVariants)
                .ThenInclude(variant => variant.Inventory)
            .Where(product => product.Id == id)
            .Select(product => new ProductResponse
            {
                Id = product.Id,
                ProductCategoryId = product.ProductCategoryId,
                ProductCategoryName = product.ProductCategory != null
                    ? product.ProductCategory.Name
                    : string.Empty,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsActive = product.IsActive,
                Variants = product.ProductVariants
                    .OrderBy(variant => variant.Sku)
                    .Select(variant => new ProductVariantResponse
                    {
                        Id = variant.Id,
                        ProductId = variant.ProductId,
                        ProductName = product.Name,
                        SizeId = variant.SizeId,
                        SizeName = variant.Size != null ? variant.Size.Name : string.Empty,
                        ColorId = variant.ColorId,
                        ColorName = variant.Color != null ? variant.Color.Name : string.Empty,
                        Sku = variant.Sku,
                        IsActive = variant.IsActive,
                        Quantity = variant.Inventory != null ? variant.Inventory.Quantity : 0,
                        MinimumQuantity = variant.Inventory != null ? variant.Inventory.MinimumQuantity : 0
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<ProductVariantSearchResponse>> SearchProductVariantsAsync(
        string? query,
        string? size,
        string? color,
        bool onlyAvailable)
    {
        var normalizedQuery = query?.Trim().ToLower();
        var normalizedSize = size?.Trim().ToLower();
        var normalizedColor = color?.Trim().ToLower();

        var variantsQuery = _context.ProductVariants
            .AsNoTracking()
            .Include(variant => variant.Product)
                .ThenInclude(product => product!.ProductCategory)
            .Include(variant => variant.Size)
            .Include(variant => variant.Color)
            .Include(variant => variant.Inventory)
            .Where(variant =>
                variant.IsActive &&
                variant.Product != null &&
                variant.Product.IsActive);

        if (!string.IsNullOrWhiteSpace(normalizedQuery))
        {
            variantsQuery = variantsQuery.Where(variant =>
                variant.Product!.Name.ToLower().Contains(normalizedQuery) ||
                (variant.Product.Description != null &&
                 variant.Product.Description.ToLower().Contains(normalizedQuery)) ||
                variant.Sku.ToLower().Contains(normalizedQuery));
        }

        if (!string.IsNullOrWhiteSpace(normalizedSize))
        {
            variantsQuery = variantsQuery.Where(variant =>
                variant.Size != null &&
                variant.Size.Name.ToLower() == normalizedSize);
        }

        if (!string.IsNullOrWhiteSpace(normalizedColor))
        {
            variantsQuery = variantsQuery.Where(variant =>
                variant.Color != null &&
                variant.Color.Name.ToLower() == normalizedColor);
        }

        if (onlyAvailable)
        {
            variantsQuery = variantsQuery.Where(variant =>
                variant.Inventory != null &&
                variant.Inventory.Quantity > 0);
        }

        return await variantsQuery
            .OrderBy(variant => variant.Product!.Name)
            .ThenBy(variant => variant.Color!.Name)
            .ThenBy(variant => variant.Size!.Name)
            .Select(variant => new ProductVariantSearchResponse
            {
                ProductVariantId = variant.Id,
                ProductId = variant.ProductId,
                ProductName = variant.Product != null ? variant.Product.Name : string.Empty,
                Description = variant.Product != null ? variant.Product.Description : null,
                CategoryName = variant.Product != null && variant.Product.ProductCategory != null
                    ? variant.Product.ProductCategory.Name
                    : string.Empty,
                Sku = variant.Sku,
                SizeName = variant.Size != null ? variant.Size.Name : string.Empty,
                ColorName = variant.Color != null ? variant.Color.Name : string.Empty,
                Price = variant.Product != null ? variant.Product.Price : 0,
                Quantity = variant.Inventory != null ? variant.Inventory.Quantity : 0,
                IsAvailable = variant.Inventory != null && variant.Inventory.Quantity > 0
            })
            .ToListAsync();
    }

    private async Task<ProductVariantResponse?> GetProductVariantByIdAsync(Guid id)
    {
        return await _context.ProductVariants
            .AsNoTracking()
            .Include(variant => variant.Product)
            .Include(variant => variant.Size)
            .Include(variant => variant.Color)
            .Include(variant => variant.Inventory)
            .Where(variant => variant.Id == id)
            .Select(variant => new ProductVariantResponse
            {
                Id = variant.Id,
                ProductId = variant.ProductId,
                ProductName = variant.Product != null ? variant.Product.Name : string.Empty,
                SizeId = variant.SizeId,
                SizeName = variant.Size != null ? variant.Size.Name : string.Empty,
                ColorId = variant.ColorId,
                ColorName = variant.Color != null ? variant.Color.Name : string.Empty,
                Sku = variant.Sku,
                IsActive = variant.IsActive,
                Quantity = variant.Inventory != null ? variant.Inventory.Quantity : 0,
                MinimumQuantity = variant.Inventory != null ? variant.Inventory.MinimumQuantity : 0
            })
            .FirstOrDefaultAsync();
    }
}
