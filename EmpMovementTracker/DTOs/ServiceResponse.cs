using System.ComponentModel.DataAnnotations;

namespace EmpMovementTracker.DTOs;

public partial class ServiceResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}