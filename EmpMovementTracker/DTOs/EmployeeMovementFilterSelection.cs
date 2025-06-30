namespace EmpMovementTracker.DTOs;

public partial class EmployeeMovementFilterSelection
{
    public List<string>? ShiftGroupIds { get; set; }
    public List<string>? WorkCells { get; set; }
    public List<string>? DepartmentCodes { get; set; }
    public List<string>? Departments { get; set; }
    public List<string>? BuildingIds { get; set; }
    public List<string>? Buildings { get; set; }
}
