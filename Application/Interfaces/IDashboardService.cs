using ProjectNetIa.Application.DTOs.Dashboard;

namespace ProjectNetIa.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync();
}
