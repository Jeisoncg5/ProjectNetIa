using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Application.DTOs.Inventory;
using ProjectNetIa.Application.Interfaces;
using ProjectNetIa.Domain.Entities;
using ProjectNetIa.Infrastructure.Data;

namespace ProjectNetIa.Infrastructure.Services;

public sealed class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _context;

    public InventoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<InventoryResponse>> GetInventoryAsync()
    {
        return await _context.Inventories
            .AsNoTracking()
            .Include(inventory => inventory.ProductVariant)
                .ThenInclude(variant => variant!.Product)
            .Include(inventory => inventory.ProductVariant)
                .ThenInclude(variant => variant!.Size)
            .Include(inventory => inventory.ProductVariant)
                .ThenInclude(variant => variant!.Color)
            .OrderBy(inventory => inventory.ProductVariant!.Product!.Name)
            .ThenBy(inventory => inventory.ProductVariant!.Sku)
            .Select(inventory => new InventoryResponse
            {
                InventoryId = inventory.Id,
                ProductVariantId = inventory.ProductVariantId,
                ProductName = inventory.ProductVariant != null && inventory.ProductVariant.Product != null
                    ? inventory.ProductVariant.Product.Name
                    : string.Empty,
                Sku = inventory.ProductVariant != null
                    ? inventory.ProductVariant.Sku
                    : string.Empty,
                SizeName = inventory.ProductVariant != null && inventory.ProductVariant.Size != null
                    ? inventory.ProductVariant.Size.Name
                    : string.Empty,
                ColorName = inventory.ProductVariant != null && inventory.ProductVariant.Color != null
                    ? inventory.ProductVariant.Color.Name
                    : string.Empty,
                Quantity = inventory.Quantity,
                MinimumQuantity = inventory.MinimumQuantity,
                IsLowStock = inventory.Quantity <= inventory.MinimumQuantity
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InventoryResponse>> GetLowStockAsync()
    {
        return await _context.Inventories
            .AsNoTracking()
            .Include(inventory => inventory.ProductVariant)
                .ThenInclude(variant => variant!.Product)
            .Include(inventory => inventory.ProductVariant)
                .ThenInclude(variant => variant!.Size)
            .Include(inventory => inventory.ProductVariant)
                .ThenInclude(variant => variant!.Color)
            .Where(inventory => inventory.Quantity <= inventory.MinimumQuantity)
            .OrderBy(inventory => inventory.Quantity)
            .Select(inventory => new InventoryResponse
            {
                InventoryId = inventory.Id,
                ProductVariantId = inventory.ProductVariantId,
                ProductName = inventory.ProductVariant != null && inventory.ProductVariant.Product != null
                    ? inventory.ProductVariant.Product.Name
                    : string.Empty,
                Sku = inventory.ProductVariant != null
                    ? inventory.ProductVariant.Sku
                    : string.Empty,
                SizeName = inventory.ProductVariant != null && inventory.ProductVariant.Size != null
                    ? inventory.ProductVariant.Size.Name
                    : string.Empty,
                ColorName = inventory.ProductVariant != null && inventory.ProductVariant.Color != null
                    ? inventory.ProductVariant.Color.Name
                    : string.Empty,
                Quantity = inventory.Quantity,
                MinimumQuantity = inventory.MinimumQuantity,
                IsLowStock = inventory.Quantity <= inventory.MinimumQuantity
            })
            .ToListAsync();
    }

    public async Task<InventoryResponse?> GetInventoryByVariantIdAsync(Guid productVariantId)
    {
        return await _context.Inventories
            .AsNoTracking()
            .Include(inventory => inventory.ProductVariant)
                .ThenInclude(variant => variant!.Product)
            .Include(inventory => inventory.ProductVariant)
                .ThenInclude(variant => variant!.Size)
            .Include(inventory => inventory.ProductVariant)
                .ThenInclude(variant => variant!.Color)
            .Where(inventory => inventory.ProductVariantId == productVariantId)
            .Select(inventory => new InventoryResponse
            {
                InventoryId = inventory.Id,
                ProductVariantId = inventory.ProductVariantId,
                ProductName = inventory.ProductVariant != null && inventory.ProductVariant.Product != null
                    ? inventory.ProductVariant.Product.Name
                    : string.Empty,
                Sku = inventory.ProductVariant != null
                    ? inventory.ProductVariant.Sku
                    : string.Empty,
                SizeName = inventory.ProductVariant != null && inventory.ProductVariant.Size != null
                    ? inventory.ProductVariant.Size.Name
                    : string.Empty,
                ColorName = inventory.ProductVariant != null && inventory.ProductVariant.Color != null
                    ? inventory.ProductVariant.Color.Name
                    : string.Empty,
                Quantity = inventory.Quantity,
                MinimumQuantity = inventory.MinimumQuantity,
                IsLowStock = inventory.Quantity <= inventory.MinimumQuantity
            })
            .FirstOrDefaultAsync();
    }

    public async Task<InventoryResponse> AdjustInventoryAsync(AdjustInventoryRequest request)
    {
        if (request.ProductVariantId == Guid.Empty)
        {
            throw new InvalidOperationException("La variante del producto es obligatoria.");
        }

        if (request.Quantity == 0)
        {
            throw new InvalidOperationException("La cantidad del ajuste no puede ser cero.");
        }

        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(inventory =>
                inventory.ProductVariantId == request.ProductVariantId);

        if (inventory is null)
        {
            throw new InvalidOperationException("No existe inventario para la variante indicada.");
        }

        var newQuantity = inventory.Quantity + request.Quantity;

        if (newQuantity < 0)
        {
            throw new InvalidOperationException("El ajuste no puede dejar el inventario en negativo.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        inventory.Quantity = newQuantity;
        inventory.UpdatedAt = DateTime.UtcNow;

        const int manualAdjustmentMovementTypeId = 2;

        var movement = new InventoryMovement
        {
            ProductVariantId = request.ProductVariantId,
            InventoryMovementTypeId = manualAdjustmentMovementTypeId,
            Quantity = request.Quantity,
            Description = request.Description?.Trim() ?? "Ajuste manual de inventario.",
            CreatedAt = DateTime.UtcNow
        };

        _context.InventoryMovements.Add(movement);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var updatedInventory = await GetInventoryByVariantIdAsync(request.ProductVariantId);

        if (updatedInventory is null)
        {
            throw new InvalidOperationException("El inventario fue actualizado, pero no pudo ser consultado.");
        }

        return updatedInventory;
    }
}
