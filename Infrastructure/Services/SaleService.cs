using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Application.DTOs.Sales;
using ProjectNetIa.Application.Interfaces;
using ProjectNetIa.Domain.Entities;
using ProjectNetIa.Infrastructure.Data;

namespace ProjectNetIa.Infrastructure.Services;

public sealed class SaleService : ISaleService
{
    private readonly ApplicationDbContext _context;

    private const int CompletedSaleStatusId = 2;
    private const int IssuedInvoiceStatusId = 1;
    private const int SaleInventoryMovementTypeId = 3;

    public SaleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SaleResponse> CreateSaleAsync(CreateSaleRequest request)
    {
        if (request.Items.Count == 0)
        {
            throw new InvalidOperationException("La venta debe tener al menos un producto.");
        }

        var saleOriginExists = await _context.SaleOrigins
            .AnyAsync(origin => origin.Id == request.SaleOriginId);

        if (!saleOriginExists)
        {
            throw new InvalidOperationException("El origen de la venta no existe.");
        }

        if (request.CustomerId.HasValue)
        {
            var customerExists = await _context.Customers
                .AnyAsync(customer => customer.Id == request.CustomerId.Value);

            if (!customerExists)
            {
                throw new InvalidOperationException("El cliente no existe.");
            }
        }

        var groupedItems = request.Items
            .GroupBy(item => item.ProductVariantId)
            .Select(group => new
            {
                ProductVariantId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .ToList();

        if (groupedItems.Any(item => item.Quantity <= 0))
        {
            throw new InvalidOperationException("La cantidad de cada producto debe ser mayor que cero.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var sale = new Sale
        {
            CustomerId = request.CustomerId,
            SaleOriginId = request.SaleOriginId,
            SaleStatusId = CompletedSaleStatusId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Sales.Add(sale);

        foreach (var item in groupedItems)
        {
            var variant = await _context.ProductVariants
                .Include(variant => variant.Product)
                .Include(variant => variant.Inventory)
                .FirstOrDefaultAsync(variant => variant.Id == item.ProductVariantId);

            if (variant is null)
            {
                throw new InvalidOperationException("Una de las variantes del producto no existe.");
            }

            if (!variant.IsActive)
            {
                throw new InvalidOperationException($"La variante con SKU {variant.Sku} esta inactiva.");
            }

            if (variant.Product is null || !variant.Product.IsActive)
            {
                throw new InvalidOperationException($"El producto asociado al SKU {variant.Sku} no esta activo.");
            }

            if (variant.Inventory is null)
            {
                throw new InvalidOperationException($"No existe inventario para el SKU {variant.Sku}.");
            }

            if (variant.Inventory.Quantity < item.Quantity)
            {
                throw new InvalidOperationException(
                    $"No hay stock suficiente para el SKU {variant.Sku}. Stock actual: {variant.Inventory.Quantity}."
                );
            }

            variant.Inventory.Quantity -= item.Quantity;
            variant.Inventory.UpdatedAt = DateTime.UtcNow;

            var saleItem = new SaleItem
            {
                SaleId = sale.Id,
                ProductVariantId = variant.Id,
                Quantity = item.Quantity,
                UnitPrice = variant.Product.Price
            };

            _context.SaleItems.Add(saleItem);

            var movement = new InventoryMovement
            {
                ProductVariantId = variant.Id,
                InventoryMovementTypeId = SaleInventoryMovementTypeId,
                Quantity = -item.Quantity,
                Description = $"Salida por venta. SKU: {variant.Sku}",
                CreatedAt = DateTime.UtcNow
            };

            _context.InventoryMovements.Add(movement);
        }

        var invoiceNumber = await GenerateInvoiceNumberAsync();

        var invoice = new Invoice
        {
            SaleId = sale.Id,
            InvoiceStatusId = IssuedInvoiceStatusId,
            InvoiceNumber = invoiceNumber,
            CreatedAt = DateTime.UtcNow
        };

        _context.Invoices.Add(invoice);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var createdSale = await GetSaleByIdAsync(sale.Id);

        if (createdSale is null)
        {
            throw new InvalidOperationException("La venta fue creada, pero no pudo ser consultada.");
        }

        return createdSale;
    }

    public async Task<IReadOnlyList<SaleResponse>> GetSalesAsync()
    {
        return await _context.Sales
            .AsNoTracking()
            .Include(sale => sale.SaleOrigin)
            .Include(sale => sale.SaleStatus)
            .Include(sale => sale.Invoice)
            .Include(sale => sale.Items)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant!.Product)
            .Include(sale => sale.Items)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant!.Size)
            .Include(sale => sale.Items)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant!.Color)
            .OrderByDescending(sale => sale.CreatedAt)
            .Select(sale => new SaleResponse
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                SaleOriginName = sale.SaleOrigin != null ? sale.SaleOrigin.Name : string.Empty,
                SaleStatusName = sale.SaleStatus != null ? sale.SaleStatus.Name : string.Empty,
                CreatedAt = sale.CreatedAt,
                InvoiceNumber = sale.Invoice != null ? sale.Invoice.InvoiceNumber : string.Empty,
                Items = sale.Items.Select(item => new SaleItemResponse
                {
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.ProductVariant != null && item.ProductVariant.Product != null
                        ? item.ProductVariant.Product.Name
                        : string.Empty,
                    Sku = item.ProductVariant != null ? item.ProductVariant.Sku : string.Empty,
                    SizeName = item.ProductVariant != null && item.ProductVariant.Size != null
                        ? item.ProductVariant.Size.Name
                        : string.Empty,
                    ColorName = item.ProductVariant != null && item.ProductVariant.Color != null
                        ? item.ProductVariant.Color.Name
                        : string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Subtotal = item.Quantity * item.UnitPrice
                }).ToList(),
                Total = sale.Items.Sum(item => item.Quantity * item.UnitPrice)
            })
            .ToListAsync();
    }

    public async Task<SaleResponse?> GetSaleByIdAsync(Guid id)
    {
        return await _context.Sales
            .AsNoTracking()
            .Include(sale => sale.SaleOrigin)
            .Include(sale => sale.SaleStatus)
            .Include(sale => sale.Invoice)
            .Include(sale => sale.Items)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant!.Product)
            .Include(sale => sale.Items)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant!.Size)
            .Include(sale => sale.Items)
                .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant!.Color)
            .Where(sale => sale.Id == id)
            .Select(sale => new SaleResponse
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                SaleOriginName = sale.SaleOrigin != null ? sale.SaleOrigin.Name : string.Empty,
                SaleStatusName = sale.SaleStatus != null ? sale.SaleStatus.Name : string.Empty,
                CreatedAt = sale.CreatedAt,
                InvoiceNumber = sale.Invoice != null ? sale.Invoice.InvoiceNumber : string.Empty,
                Items = sale.Items.Select(item => new SaleItemResponse
                {
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.ProductVariant != null && item.ProductVariant.Product != null
                        ? item.ProductVariant.Product.Name
                        : string.Empty,
                    Sku = item.ProductVariant != null ? item.ProductVariant.Sku : string.Empty,
                    SizeName = item.ProductVariant != null && item.ProductVariant.Size != null
                        ? item.ProductVariant.Size.Name
                        : string.Empty,
                    ColorName = item.ProductVariant != null && item.ProductVariant.Color != null
                        ? item.ProductVariant.Color.Name
                        : string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Subtotal = item.Quantity * item.UnitPrice
                }).ToList(),
                Total = sale.Items.Sum(item => item.Quantity * item.UnitPrice)
            })
            .FirstOrDefaultAsync();
    }

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var invoiceCount = await _context.Invoices.CountAsync() + 1;
        return $"FAC-{invoiceCount:000000}";
    }
}
