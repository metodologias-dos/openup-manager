namespace OpenUpMan.Services;

public interface IDashboardService
{
    Task<DashboardData?> GetDashboardDataAsync(int projectId, CancellationToken ct = default);
}

