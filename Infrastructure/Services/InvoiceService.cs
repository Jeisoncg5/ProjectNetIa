using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Application.DTOs.Invoices;
using ProjectNetIa.Application.Interfaces;
using ProjectNetIa.Infrastructure.Data;

namespace ProjectNetIa.Infrastructure.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly ApplicationDbContext _context;

    public InvoiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<InvoiceResponse>> GetInvoicesAsync()
    {
        return await _context.Invoices
            .AsNoTracking()
            .Include(invoice => invoice.InvoiceStatus)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.SaleOrigin)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant!.Product)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant!.Size)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant!.Color)
            .OrderByDescending(invoice => invoice.CreatedAt)
            .Select(invoice => new InvoiceResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                SaleId = invoice.SaleId,
                InvoiceStatusName = invoice.InvoiceStatus != null
                    ? invoice.InvoiceStatus.Name
                    : string.Empty,
                SaleOriginName = invoice.Sale != null && invoice.Sale.SaleOrigin != null
                    ? invoice.Sale.SaleOrigin.Name
                    : string.Empty,
                CreatedAt = invoice.CreatedAt,
                Total = invoice.Sale != null
                    ? invoice.Sale.Items.Sum(item => item.Quantity * item.UnitPrice)
                    : 0,
                Items = invoice.Sale != null
                    ? invoice.Sale.Items.Select(item => new InvoiceItemResponse
                    {
                        ProductName = item.ProductVariant != null && item.ProductVariant.Product != null
                            ? item.ProductVariant.Product.Name
                            : string.Empty,
                        Sku = item.ProductVariant != null
                            ? item.ProductVariant.Sku
                            : string.Empty,
                        SizeName = item.ProductVariant != null && item.ProductVariant.Size != null
                            ? item.ProductVariant.Size.Name
                            : string.Empty,
                        ColorName = item.ProductVariant != null && item.ProductVariant.Color != null
                            ? item.ProductVariant.Color.Name
                            : string.Empty,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Quantity * item.UnitPrice
                    }).ToList()
                    : new List<InvoiceItemResponse>()
            })
            .ToListAsync();
    }

    public async Task<InvoiceResponse?> GetInvoiceByIdAsync(Guid id)
    {
        return await _context.Invoices
            .AsNoTracking()
            .Include(invoice => invoice.InvoiceStatus)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.SaleOrigin)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant!.Product)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant!.Size)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant!.Color)
            .Where(invoice => invoice.Id == id)
            .Select(invoice => new InvoiceResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                SaleId = invoice.SaleId,
                InvoiceStatusName = invoice.InvoiceStatus != null
                    ? invoice.InvoiceStatus.Name
                    : string.Empty,
                SaleOriginName = invoice.Sale != null && invoice.Sale.SaleOrigin != null
                    ? invoice.Sale.SaleOrigin.Name
                    : string.Empty,
                CreatedAt = invoice.CreatedAt,
                Total = invoice.Sale != null
                    ? invoice.Sale.Items.Sum(item => item.Quantity * item.UnitPrice)
                    : 0,
                Items = invoice.Sale != null
                    ? invoice.Sale.Items.Select(item => new InvoiceItemResponse
                    {
                        ProductName = item.ProductVariant != null && item.ProductVariant.Product != null
                            ? item.ProductVariant.Product.Name
                            : string.Empty,
                        Sku = item.ProductVariant != null
                            ? item.ProductVariant.Sku
                            : string.Empty,
                        SizeName = item.ProductVariant != null && item.ProductVariant.Size != null
                            ? item.ProductVariant.Size.Name
                            : string.Empty,
                        ColorName = item.ProductVariant != null && item.ProductVariant.Color != null
                            ? item.ProductVariant.Color.Name
                            : string.Empty,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Quantity * item.UnitPrice
                    }).ToList()
                    : new List<InvoiceItemResponse>()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<InvoiceResponse?> GetInvoiceByNumberAsync(string invoiceNumber)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
        {
            return null;
        }

        var normalizedInvoiceNumber = invoiceNumber.Trim().ToUpperInvariant();

        return await _context.Invoices
            .AsNoTracking()
            .Include(invoice => invoice.InvoiceStatus)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.SaleOrigin)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant!.Product)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant!.Size)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant!.Color)
            .Where(invoice => invoice.InvoiceNumber.ToUpper() == normalizedInvoiceNumber)
            .Select(invoice => new InvoiceResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                SaleId = invoice.SaleId,
                InvoiceStatusName = invoice.InvoiceStatus != null
                    ? invoice.InvoiceStatus.Name
                    : string.Empty,
                SaleOriginName = invoice.Sale != null && invoice.Sale.SaleOrigin != null
                    ? invoice.Sale.SaleOrigin.Name
                    : string.Empty,
                CreatedAt = invoice.CreatedAt,
                Total = invoice.Sale != null
                    ? invoice.Sale.Items.Sum(item => item.Quantity * item.UnitPrice)
                    : 0,
                Items = invoice.Sale != null
                    ? invoice.Sale.Items.Select(item => new InvoiceItemResponse
                    {
                        ProductName = item.ProductVariant != null && item.ProductVariant.Product != null
                            ? item.ProductVariant.Product.Name
                            : string.Empty,
                        Sku = item.ProductVariant != null
                            ? item.ProductVariant.Sku
                            : string.Empty,
                        SizeName = item.ProductVariant != null && item.ProductVariant.Size != null
                            ? item.ProductVariant.Size.Name
                            : string.Empty,
                        ColorName = item.ProductVariant != null && item.ProductVariant.Color != null
                            ? item.ProductVariant.Color.Name
                            : string.Empty,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Quantity * item.UnitPrice
                    }).ToList()
                    : new List<InvoiceItemResponse>()
            })
            .FirstOrDefaultAsync();
    }
}
