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
        public async Task<DashboardKeyMetrics> KeyMetricsData(DashboardFilter dto)
        {
            if (dto.Date == null)
                throw new ArgumentNullException(nameof(dto.Date), "Date is missing. Please select a date before applying the filter.");

            var movements = await context.EmployeeMovements.Where(f => f.Date == DateOnly.FromDateTime((DateTime)dto.Date)).ToListAsync();

            if (!string.IsNullOrEmpty(dto.BuildingId))
                movements = movements.Where(a => a.BuildingId == dto.BuildingId).ToList();

            var peakHourGroup = movements.GroupBy(e => new TimeOnly(e.Time.Hour, e.Time.Minute, 0)).Select(g => new { g.Key.Hour, Count = g.Count() }).OrderByDescending(g => g.Count).FirstOrDefault();

            return new DashboardKeyMetrics
            {
                TotalEmployee = movements.Select(f => f.PersonId).Distinct().Count(),
                TotalTurnstileAccess = movements.Count,
                PeakHour = peakHourGroup != null ? new TimeOnly(peakHourGroup.Hour, 0, 0).ToString("h tt") : "N/A"
            };
        }

        // Get the number of turnstile movement over time on a certain date.
        public async Task<List<TimeSeriesChartSeries.TimeValue>> TurnstileActivityOverTime(DashboardFilter dto)
        {
            if (dto.Date == null)
                throw new ArgumentNullException(nameof(dto.Date), "Date is missing. Please select a date before applying the filter.");

            var movements = await context.EmployeeMovements.Where(e => e.Date == DateOnly.FromDateTime((DateTime)dto.Date)).Select(x => new { x.Id, x.Date, x.Time, x.BuildingId }).ToListAsync();

            if (!string.IsNullOrEmpty(dto.BuildingId))
                movements = movements.Where(a => a.BuildingId == dto.BuildingId).ToList();

            var chartData = movements
                .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, e.Time.Hour, e.Time.Minute, 0))
                .Select(g => new TimeSeriesChartSeries.TimeValue(g.Key, g.Count()))
                .ToList();

            return chartData;
        }

        // Get the number of turnstile movement for each building on a certain date.
        public async Task<Dictionary<string, double>> TurnstileMovementEachBuilding(DashboardFilter dto)
        {
            if (dto.Date == null)
                throw new ArgumentNullException(nameof(dto.Date), "Date is missing. Please select a date before applying the filter.");

            var movements = await context.EmployeeMovements.Where(e => e.Date == DateOnly.FromDateTime((DateTime)dto.Date)).Select(x => new { x.Id, x.BuildingId }).ToListAsync();

            if (!string.IsNullOrEmpty(dto.BuildingId))
                movements = movements.Where(a => a.BuildingId == dto.BuildingId).ToList();

            var chartData = movements
                .GroupBy(e => e.BuildingId)
                .OrderByDescending(g => g.Key)
                .ToDictionary(g => string.IsNullOrWhiteSpace(g.Key) ? "Other" : g.Key, g => (double)g.Count());

            return chartData;
        }
    }
}