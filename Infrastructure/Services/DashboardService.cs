using Microsoft.EntityFrameworkCore;
using ProjectNetIa.Application.DTOs.Dashboard;
using ProjectNetIa.Application.Interfaces;
using ProjectNetIa.Infrastructure.Data;

namespace ProjectNetIa.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    private const int ChatbotSaleOriginId = 2;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var totalProducts = await _context.Products
            .AsNoTracking()
            .CountAsync(product => product.IsActive);

        var totalVariants = await _context.ProductVariants
            .AsNoTracking()
            .CountAsync(variant => variant.IsActive);

        var totalInventoryUnits = await _context.Inventories
            .AsNoTracking()
            .SumAsync(inventory => inventory.Quantity);

        var lowStockItems = await _context.Inventories
            .AsNoTracking()
            .CountAsync(inventory => inventory.Quantity <= inventory.MinimumQuantity);

        var salesToday = await _context.Sales
            .AsNoTracking()
            .CountAsync(sale => sale.CreatedAt >= today && sale.CreatedAt < tomorrow);

        var salesTodayTotal = await _context.Sales
            .AsNoTracking()
            .Where(sale => sale.CreatedAt >= today && sale.CreatedAt < tomorrow)
            .SelectMany(sale => sale.Items)
            .SumAsync(item => item.Quantity * item.UnitPrice);

        var chatbotSales = await _context.Sales
            .AsNoTracking()
            .CountAsync(sale => sale.SaleOriginId == ChatbotSaleOriginId);

        var latestInvoices = await _context.Invoices
            .AsNoTracking()
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.SaleOrigin)
            .Include(invoice => invoice.Sale)
                .ThenInclude(sale => sale!.Items)
            .OrderByDescending(invoice => invoice.CreatedAt)
            .Take(5)
            .Select(invoice => new DashboardInvoiceResponse
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                CreatedAt = invoice.CreatedAt,
                SaleOriginName = invoice.Sale != null && invoice.Sale.SaleOrigin != null
                    ? invoice.Sale.SaleOrigin.Name
                    : string.Empty,
                Total = invoice.Sale != null
                    ? invoice.Sale.Items.Sum(item => item.Quantity * item.UnitPrice)
                    : 0
            })
            .ToListAsync();

        return new DashboardSummaryResponse
        {
            TotalProducts = totalProducts,
            TotalVariants = totalVariants,
            TotalInventoryUnits = totalInventoryUnits,
            LowStockItems = lowStockItems,
            SalesToday = salesToday,
            SalesTodayTotal = salesTodayTotal,
            ChatbotSales = chatbotSales,
            LatestInvoices = latestInvoices
        };
    }
}
