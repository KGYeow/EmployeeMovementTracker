using EmpMovementTracker.DTOs;
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

        //public async Task<List<EmployeeTimeTracking>> PresentEmployeeMovementList(EmployeeMovementFilter dto)
        //{
        //    var dateOnly = DateOnly.FromDateTime((DateTime)dto.Date);

        //    var list = await context.EmployeeTimeTrackings
        //        .Where(a => a.Date == dateOnly)
        //        .ToListAsync();

        //    // Apply filters
        //    if (!string.IsNullOrEmpty(dto.Name))
        //        list = list.Where(a => EF.Functions.Like(a.Name, $"%{dto.Name}%")).ToList();

        //    if (!string.IsNullOrEmpty(dto.WorkCell))
        //        list = list.Where(a => a.WorkCell == dto.WorkCell).ToList();

        //    if (!string.IsNullOrEmpty(dto.Department))
        //        list = list.Where(a => a.Department == dto.Department).ToList();

        //    // Populate InitialTime and FinalTime from EmployeeMovements
        //    foreach (var item in list)
        //    {
        //        var movements = await context.EmployeeMovements
        //            .Where(m => m.PersonId == item.PersonId && m.Date == dateOnly)
        //            .OrderBy(m => m.Time)
        //            .ToListAsync();

        //        if (movements.Any())
        //        {
        //            item.InitialTime = movements.First().Time;
        //            item.FinalTime = movements.Last().Time;
        //        }
        //    }

        //    return list;
        //}


        // Get the employee info of an employee on a certain date.
        public async Task<EmployeeTimeTracking> EmployeeInfo(string personId, DateOnly date)
        {
            var employeeInfo = await context.EmployeeTimeTrackings.Where(a => a.PersonId == personId & a.Date == date).FirstOrDefaultAsync();

            return employeeInfo ?? new EmployeeTimeTracking();
        }
    }
}