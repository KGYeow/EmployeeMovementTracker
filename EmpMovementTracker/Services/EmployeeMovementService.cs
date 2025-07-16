using EmpMovementTracker.DTOs;
using EmpMovementTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        // Get the specific employee turnstile movement of an employee.
        public async Task<EmployeeMovementEdit> Edit(int id)
        {
            var empMovement = await context.EmployeeMovements.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (empMovement == null)
                throw new Exception("Employee turnstile movement data not found.");

            var editDto = new EmployeeMovementEdit
            {
                PersonId = empMovement.PersonId,
                Name = empMovement.Name,
                Date = empMovement.Date.ToDateTime(TimeOnly.MinValue),
                Time = empMovement.Time.ToTimeSpan(),
                Station = empMovement.Station,
                WorkCell = empMovement.WorkCell,
                DepartmentCode = empMovement.DepartmentCode,
                Department = empMovement.Department,
                ShiftGroupId = empMovement.ShiftGroupId,
                ShiftGroup = empMovement.ShiftGroup,
                BuildingId = empMovement.BuildingId,
                Building = empMovement.Building,
            };
            return editDto;
        }

        // Update the existing employee turnstile movement.
        public async Task Update(EmployeeMovementEdit dto, int id)
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
                    movementInfo.DateTime = (dto.Date + dto.Time) ?? DateTime.MinValue;
                    movementInfo.Date = DateOnly.FromDateTime(dto.Date ?? DateTime.MinValue);
                    movementInfo.Time = TimeOnly.FromTimeSpan(dto.Time ?? TimeSpan.MinValue);
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
        // GET: EmployeeMovement/Create
        //public async Task<EmployeeMovementCreate> Create(EmployeeMovementCreate dto)
        //{
        //    await GetInitializedSelections();
        //    return new EmployeeMovementCreate();

        //}

        // POST: EmployeeMovement/Create
        public async Task Create(EmployeeMovementCreate dto)
        {

            var safeDate = dto.Date ?? DateTime.Today;
            var safeTime = dto.Time ?? TimeSpan.Zero;
            var safeDateTime = safeDate.Date + safeTime;
            var newEmployee = new EmployeeMovement
            {
                PersonId = dto.PersonId,
                Name = dto.Name,
                Station = dto.Station,
                WorkCell = dto.WorkCell,
                DepartmentCode = dto.DepartmentCode,
                Department = dto.Department,
                ShiftGroupId = dto.ShiftGroupId,
                ShiftGroup = dto.ShiftGroup,
                BuildingId = dto.BuildingId,
                Building = dto.Building,
                DateTime = safeDateTime,
                Date = DateOnly.FromDateTime(safeDateTime),
                Time = TimeOnly.FromTimeSpan(dto.Time ?? TimeSpan.MinValue),

            };

            await context.EmployeeMovements.AddAsync(newEmployee);


            // Check if a time tracking record already exists
            var existingTimeTracking = await context.EmployeeTimeTrackings
                .FirstOrDefaultAsync(t =>
                    t.PersonId == dto.PersonId &&
                    t.Date == DateOnly.FromDateTime(safeDate));

            if (existingTimeTracking == null)
            {
                var newTimeTracking = new EmployeeTimeTracking
                {
                    PersonId = dto.PersonId,
                    Name = dto.Name,
                    Date = DateOnly.FromDateTime(safeDate),
                    WorkCell = dto.WorkCell,
                    Department = dto.Department,
                    ShiftGroup = dto.ShiftGroup,
                    Building = dto.Building
                };

                await context.EmployeeTimeTrackings.AddAsync(newTimeTracking);
            }

            await context.SaveChangesAsync();

        }
        // Delete the existing employee turnstile movement.
        public async Task Delete(int id)
        {
            var existingEmpMovement = context.EmployeeMovements.Where(a => a.Id == id).FirstOrDefault();
            context.EmployeeMovements.Remove(existingEmpMovement);
            await context.SaveChangesAsync();

            var any = context.EmployeeMovements
                .Where(a => a.PersonId == existingEmpMovement.PersonId && a.Date == existingEmpMovement.Date)
                .Any();

            if (!any)
            {
                var existingEmpTimeTracking = context.EmployeeTimeTrackings
                    .Where(a => a.PersonId == existingEmpMovement.PersonId && a.Date == existingEmpMovement.Date)
                    .FirstOrDefault();

                context.EmployeeTimeTrackings.Remove(existingEmpTimeTracking);
                await context.SaveChangesAsync();
            }
        }
    }
}