using EmpMovementTracker.DTOs.EmployeeTurnstileMovement;
using EmpMovementTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpMovementTracker.Services
{
    public class EmployeeTimeTrackingService : BaseService
    {
        public EmployeeTimeTrackingService(TurnstileDbContext context) : base(context)
        {
        }

        // Get the list of present employee movement based on the date.
        public async Task<List<EmployeeTimeTracking>> PresentEmployeeMovementList(EmployeeMovementFilter dto)
        {
            var list = context.EmployeeTimeTrackings.Where(a => a.Date == DateOnly.FromDateTime((DateTime)dto.Date)).AsQueryable();

            // Filter the list of present employee movements.
            if (!string.IsNullOrEmpty(dto.Name))
                list = list.Where(a => EF.Functions.Like(a.Name, $"%{dto.Name}%"));

            if (!string.IsNullOrEmpty(dto.WorkCell))
                list = list.Where(a => a.WorkCell == dto.WorkCell);

            if (!string.IsNullOrEmpty(dto.Department))
                list = list.Where(a => a.Department == dto.Department);

            return await list.ToListAsync();
        }

        // Get the employee info of an employee on a certain date.
        public async Task<EmployeeTimeTracking> EmployeeInfo(string personId, DateOnly date)
        {
            var employeeInfo = await context.EmployeeTimeTrackings.Where(a => a.PersonId == personId & a.Date == date).FirstOrDefaultAsync();

            return employeeInfo ?? new EmployeeTimeTracking();
        }
    }
}