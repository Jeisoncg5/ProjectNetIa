namespace ProjectNetIa.Application.DTOs.Dashboard;

public sealed class DashboardSummaryResponse
{
    public int TotalProducts { get; set; }

    public int TotalVariants { get; set; }

    public int TotalInventoryUnits { get; set; }

    public int LowStockItems { get; set; }

    public int SalesToday { get; set; }

    public decimal SalesTodayTotal { get; set; }

    public int ChatbotSales { get; set; }

    public List<DashboardInvoiceResponse> LatestInvoices { get; set; } = new();
}
