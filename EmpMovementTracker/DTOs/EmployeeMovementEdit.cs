using System.ComponentModel.DataAnnotations;

namespace EmpMovementTracker.DTOs;

public partial class EmployeeMovementEdit
{
    public string PersonId { get; set; } = null!;

    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Date is required")]
    public DateTime? Date { get; set; }

    [Required(ErrorMessage = "Time is required")]
    public TimeSpan? Time { get; set; }

    [Required(ErrorMessage = "Station is required")]
    public string Station { get; set; } = null!;

    [Required(ErrorMessage = "Work cell is required")]
    public string WorkCell { get; set; } = null!;

    [Required(ErrorMessage = "Department code is required")]
    public string DepartmentCode { get; set; } = null!;

    [Required(ErrorMessage = "Department is required")]
    public string Department { get; set; } = null!;

    [Required(ErrorMessage = "Shift group ID is required")]
    public string ShiftGroupId { get; set; } = null!;

    [Required(ErrorMessage = "Shift group is required")]
    public int ShiftGroup { get; set; }

    [Required(ErrorMessage = "Building ID is required")]
    public string BuildingId { get; set; } = null!;

    [Required(ErrorMessage = "Building is required")]
    public string Building { get; set; } = null!;
}