using EmpMovementTracker.DTOs;
using EmpMovementTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace EmpMovementTracker.Services
{
    public class EmployeeMovementService : BaseService
    {
        public EmployeeMovementService(TurnstileDbContext context) : base(context)
        {
        }

        // Get the dashboard data for the list of present employee movement based on the date.
        public EmployeeMovementDashboard DashboardData(DateTime? date)
        {
            var list = context.EmployeeMovements.Where(f => f.Date == DateOnly.FromDateTime((DateTime)date)).AsQueryable();

            return new EmployeeMovementDashboard
            {
                TotalEmployee = list.Select(f => f.PersonId).Distinct().Count(),
                TotalTurnstileAccess = list.Count()
            };
        }

        // Get the initialized & populated neccessary selections.
        public async Task<EmployeeMovementFilterSelection> GetInitializedSelections()
        {
            var list = context.EmployeeMovements.AsQueryable();

            return new EmployeeMovementFilterSelection
            {
                ShiftGroupIds = await list
                    .Select(e => string.IsNullOrEmpty(e.ShiftGroupId) ? "N/A" : e.ShiftGroupId)
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync(),

                WorkCells = await list
                    .Select(e => string.IsNullOrEmpty(e.WorkCell) ? "N/A" : e.WorkCell)
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync(),

                Departments = await list
                    .Select(e => string.IsNullOrEmpty(e.Department) ? "N/A" : e.Department)
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync(),

                DepartmentCodes = await list
                    .Select(e => string.IsNullOrEmpty(e.DepartmentCode) ? "N/A" : e.DepartmentCode)
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync(),

                Buildings = await list
                    .Select(e => string.IsNullOrEmpty(e.Building) ? "N/A" : e.Building)
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync(),

                BuildingIds = await list
                    .Select(e => string.IsNullOrEmpty(e.BuildingId) ? "N/A" : e.BuildingId)
                    .Distinct()
                    .OrderBy(e => e)
                    .ToListAsync(),
            };
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
        public EmployeeMovement EmployeeInfo(string personId, DateTime date)
        {
            var employeeInfo = context.EmployeeMovements
                .Where(a => a.PersonId == personId & a.Date == DateOnly.FromDateTime((DateTime)date))
                .LastOrDefault();

            return employeeInfo ?? new EmployeeMovement();
        }

        // Get the list of employee turnstile movement of an employee on a certain date.
        public async Task<List<EmployeeMovement>> EmployeeTurnstileMovementList([FromQuery] EmployeeMovementFilter dto, [FromQuery] string personId)
        {
            var list = context.EmployeeMovements.Where(a => a.PersonId == personId & a.Date == DateOnly.FromDateTime((DateTime)dto.Date))
                .Select(g => new EmployeeMovement
                {
                    Id = g.Id,
                    Date = g.Date,
                    Time = g.Time,
                    Station = g.Station,
                    DepartmentCode = g.DepartmentCode,
                    ShiftGroupId = g.ShiftGroupId,
                    ShiftGroup = g.ShiftGroup,
                    BuildingId = string.IsNullOrEmpty(g.BuildingId) ? "N/A" : g.BuildingId,
                    Building = string.IsNullOrEmpty(g.Building) ? "N/A" : g.Building,
                }).OrderBy(g => g.Time).AsQueryable();

            if (!string.IsNullOrEmpty(dto.Station))
                list = list.Where(a => EF.Functions.Like(a.Station, $"%{dto.Station}%"));

            if (!string.IsNullOrEmpty(dto.Building))
                list = list.Where(a => a.Building == dto.Building);

            return await list.ToListAsync();
        }
    }
}