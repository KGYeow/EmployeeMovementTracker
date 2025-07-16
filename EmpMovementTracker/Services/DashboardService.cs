using EmpMovementTracker.DTOs;
using EmpMovementTracker.Models;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace EmpMovementTracker.Services
{
    public class DashboardService : BaseService
    {
        public DashboardService(TurnstileDbContext context) : base(context)
        {
        }

        // Get the dashboard data for the list of present employee movement based on the date.
        public async Task<EmployeeMovementDashboard> KeyMetricsData(DateOnly date)
        {
            var list = await context.EmployeeMovements.Where(f => f.Date == date).ToListAsync();

            return new EmployeeMovementDashboard
            {
                TotalEmployee = list.Select(f => f.PersonId).Distinct().Count(),
                TotalTurnstileAccess = list.Count
            };
        }

        // Get the list of number of turnstile movement over time on a certain date.
        public async Task<List<TimeSeriesChartSeries.TimeValue>> TurnstileMovementOverTime(DateOnly date)
        {
            var movements = await context.EmployeeMovements
                .Where(e => e.Date == date)
                .Select(x => new { x.PersonId, x.Date, x.Time })
                .ToListAsync();

            var chartData = movements
                .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, e.Time.Hour, e.Time.Minute, 0))
                .Select(g => new TimeSeriesChartSeries.TimeValue(g.Key, g.Count()))
                .ToList();

            return chartData;
        }

    }
}