using System;
using System.Collections.Generic;

namespace EmpMovementTracker.Models;

public partial class EmployeeTimeTracking
{
    public int Id { get; set; }

    public string PersonId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly Date { get; set; }

    public TimeOnly InitialTime { get; set; }

    public TimeOnly FinalTime { get; set; }

    public string Building { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string WorkCell { get; set; } = null!;

    public int ShiftGroup { get; set; }
}
