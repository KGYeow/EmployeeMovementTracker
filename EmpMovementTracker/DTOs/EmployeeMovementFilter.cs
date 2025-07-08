namespace EmpMovementTracker.DTOs;

public partial class EmployeeMovementFilter
{
    public string? Name { get; set; }
    public DateTime? Date { get; set; }
    public string? Station { get; set; }
    public string? WorkCell { get; set; }
    public string? Department { get; set; }
    public string? BuildingId { get; set; }
    public string? Building { get; set; }
    public int ShiftGroup { get; set; }
}
