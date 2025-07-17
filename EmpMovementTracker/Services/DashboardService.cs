using EmpMovementTracker.DTOs.Dashboard;
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

        // Get the key metrics data for based on the date.
        public async Task<DashboardKeyMetrics> KeyMetricsData(DateOnly date)
        {
            var movements = await context.EmployeeMovements.Where(f => f.Date == date).ToListAsync();
            var peakHourGroup = movements.GroupBy(e => new TimeOnly(e.Time.Hour, e.Time.Minute, 0)).Select(g => new { g.Key.Hour, Count = g.Count() }).OrderByDescending(g => g.Count).FirstOrDefault();

            return new DashboardKeyMetrics
            {
                TotalEmployee = movements.Select(f => f.PersonId).Distinct().Count(),
                TotalTurnstileAccess = movements.Count,
                PeakHour = peakHourGroup != null ? new TimeOnly(peakHourGroup.Hour, 0, 0).ToString("h tt") : "N/A"
            };
        }

        // Get the number of turnstile movement over time on a certain date.
        public async Task<List<TimeSeriesChartSeries.TimeValue>> TurnstileActivityOverTime(DateOnly date)
        {
            var movements = await context.EmployeeMovements.Where(e => e.Date == date).Select(x => new { x.Id, x.Date, x.Time }).ToListAsync();

            var chartData = movements
                .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, e.Time.Hour, e.Time.Minute, 0))
                .Select(g => new TimeSeriesChartSeries.TimeValue(g.Key, g.Count()))
                .ToList();

            return chartData;
        }

        // Get the number of turnstile movement for each building on a certain date.
        public async Task<Dictionary<string, double>> TurnstileMovementEachBuilding(DateOnly date)
        {
            var movements = await context.EmployeeMovements.Where(e => e.Date == date).Select(x => new { x.Id, x.BuildingId }).ToListAsync();

            var chartData = movements
                .GroupBy(e => e.BuildingId)
                .OrderByDescending(g => g.Key)
                .ToDictionary(g => string.IsNullOrWhiteSpace(g.Key) ? "Other" : g.Key, g => (double)g.Count());

            return chartData;
        }
    }
}