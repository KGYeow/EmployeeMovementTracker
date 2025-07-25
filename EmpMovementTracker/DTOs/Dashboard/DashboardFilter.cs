namespace EmpMovementTracker.DTOs.Dashboard;

public partial class DashboardFilter
{
    public DateTime? Date { get; set; } = DateTime.Parse("2021-6-13");
    public string? BuildingId { get; set; } = String.Empty;
}