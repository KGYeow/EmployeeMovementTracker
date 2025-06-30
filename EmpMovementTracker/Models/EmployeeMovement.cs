using System;
using System.Collections.Generic;

namespace EmpMovementTracker.Models;

public partial class EmployeeMovement
{
    public int Id { get; set; }

    public string PersonId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public DateTime DateTime { get; set; }

    public string? Station { get; set; }

    public string WorkCell { get; set; } = null!;

    public string DepartmentCode { get; set; } = null!;

    public string? Department { get; set; }

    public string ShiftGroupId { get; set; } = null!;

    public int ShiftGroup { get; set; }

    public string BuildingId { get; set; } = null!;

    public string? Building { get; set; }
}
