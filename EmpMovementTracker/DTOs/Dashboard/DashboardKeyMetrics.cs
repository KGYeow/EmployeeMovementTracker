namespace EmpMovementTracker.DTOs.Dashboard;

public partial class DashboardKeyMetrics
{
    public int TotalEmployee { get; set; }
    public int TotalTurnstileAccess { get; set; }
    public string PeakHour { get; set; } = "N/A";
}