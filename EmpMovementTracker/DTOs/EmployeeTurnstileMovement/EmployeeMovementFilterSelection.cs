namespace EmpMovementTracker.DTOs.EmployeeTurnstileMovement;

public partial class EmployeeMovementFilterSelection
{
    public List<string> ShiftGroupIds { get; set; } = [];
    public List<string> WorkCells { get; set; } = [];
    public List<string> DepartmentCodes { get; set; } = [];
    public List<string> Departments { get; set; } = [];
    public List<string> BuildingIds { get; set; } = [];
    public List<string> Buildings { get; set; } = [];

    public Task<IEnumerable<string>> SearchShiftGroupId(string value, CancellationToken token)
    {
        // if text is null or empty, show complete list
        return Task.FromResult(string.IsNullOrEmpty(value)
            ? ShiftGroupIds
            : ShiftGroupIds.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase))
        );
    }

    public Task<IEnumerable<string>> SearchWorkCell(string value, CancellationToken token)
    {
        // if text is null or empty, show complete list
        return Task.FromResult(string.IsNullOrEmpty(value)
			? WorkCells
            : WorkCells.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase))
        );
    }

    public Task<IEnumerable<string>> SearchDepartmentCode(string value, CancellationToken token)
    {
        // if text is null or empty, show complete list
        return Task.FromResult(string.IsNullOrEmpty(value)
			? DepartmentCodes
            : DepartmentCodes.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase))
        );
    }

    public Task<IEnumerable<string>> SearchDepartment(string value, CancellationToken token)
    {
        // if text is null or empty, show complete list
        return Task.FromResult(string.IsNullOrEmpty(value)
			? Departments
            : Departments.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase))
        );
    }

    public Task<IEnumerable<string>> SearchBuildingId(string value, CancellationToken token)
    {
        // if text is null or empty, show complete list
        return Task.FromResult(string.IsNullOrEmpty(value)
			? BuildingIds
            : BuildingIds.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase))
        );
    }

    public Task<IEnumerable<string>> SearchBuilding(string value, CancellationToken token)
    {
        // if text is null or empty, show complete list
        return Task.FromResult(string.IsNullOrEmpty(value)
			? Buildings
            : Buildings.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase))
        );
    }
}