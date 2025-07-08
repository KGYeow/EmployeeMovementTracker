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

        // Get the list of employee turnstile movement of an employee on a certain date.
        public async Task<List<EmployeeMovement>> EmployeeTurnstileMovementList([FromQuery] EmployeeMovementFilter dto, [FromQuery] string personId)
        {
            var list = context.EmployeeMovements.Where(a => a.PersonId == personId & a.Date == DateOnly.FromDateTime((DateTime)dto.Date))
                .Select(g => new EmployeeMovement
                {
                    Id = g.Id,
                    Time = g.Time,
                    Station = g.Station,
                    DepartmentCode = g.DepartmentCode,
                    ShiftGroupId = g.ShiftGroupId,
                    BuildingId = string.IsNullOrEmpty(g.BuildingId) ? "N/A" : g.BuildingId,
                }).OrderBy(g => g.Time).AsQueryable();

            if (!string.IsNullOrEmpty(dto.Station))
                list = list.Where(a => EF.Functions.Like(a.Station, $"%{dto.Station}%"));

            if (!string.IsNullOrEmpty(dto.BuildingId))
                list = list.Where(a => a.BuildingId == dto.BuildingId);

            return await list.ToListAsync();
        }

        // Update the existing employee turnstile movement.
        public async void Update(EmployeeMovementEdit dto, int id)
        {
            var movementList = context.EmployeeMovements.Where(a => a.PersonId == dto.PersonId).AsQueryable();
            var timeTrackingList = context.EmployeeTimeTrackings.Where(a => a.PersonId == dto.PersonId).AsQueryable();

            foreach (var timeTrackingInfo in timeTrackingList)
            {
                timeTrackingInfo.WorkCell = dto.WorkCell;
                timeTrackingInfo.Department = dto.Department;
                timeTrackingInfo.ShiftGroup = dto.ShiftGroup;
                timeTrackingInfo.Building = dto.Building;
            }

            foreach (var movementInfo in movementList)
            {
                if (movementInfo.Id == id)
                {
                    movementInfo.DateTime = dto.DateTime;
                    movementInfo.Date = DateOnly.FromDateTime(dto.DateTime);
                    movementInfo.Time = TimeOnly.FromDateTime(dto.DateTime);
                    movementInfo.Station = dto.Station;
                    movementInfo.BuildingId = dto.BuildingId;
                }
                movementInfo.WorkCell = dto.WorkCell;
                movementInfo.DepartmentCode = dto.DepartmentCode;
                movementInfo.Department = dto.Department;
                movementInfo.ShiftGroupId = dto.ShiftGroupId;
                movementInfo.ShiftGroup = dto.ShiftGroup;
                movementInfo.Building = dto.Building;
            }

            context.EmployeeTimeTrackings.UpdateRange(timeTrackingList);
            context.EmployeeMovements.UpdateRange(movementList);
            await context.SaveChangesAsync();
        }

        // Delete the existing employee turnstile movement.
        public async Task<ServiceResponse> Delete(int id)
        {
            try
            {
                var existingEmployeeMovement = context.EmployeeMovements.Where(a => a.Id == id).FirstOrDefault();
                context.EmployeeMovements.Remove(existingEmployeeMovement);
                await context.SaveChangesAsync();

                var any = context.EmployeeMovements
                    .Where(a => a.PersonId == existingEmployeeMovement.PersonId && a.Date == existingEmployeeMovement.Date)
                    .Any();

                if (!any)
                {
                    var existingEmployeeTimeTracking = context.EmployeeTimeTrackings
                        .Where(a => a.PersonId == existingEmployeeMovement.PersonId && a.Date == existingEmployeeMovement.Date)
                        .FirstOrDefault();

                    context.EmployeeTimeTrackings.Remove(existingEmployeeTimeTracking);
                    await context.SaveChangesAsync();
                }

                return new() { Success = true, Message = "The employee turnstile movement has been deleted" };
            }
            catch (Exception ex)
            {
                return new() { Success = false, Message = ex.Message };
            }
        }
    }
}