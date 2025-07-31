using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class TechnicianForecastService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianForecastService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Analyze past TechnicianInsightLogs, average performance per tech, predict future load/performance
        public async Task<List<TechnicianForecastLog>> GenerateForecastsAsync(DateTime? startDate = null, DateTime? endDate = null, int? technicianId = null)
        {
            var query = _db.TechnicianInsightLogs.AsQueryable();
            if (startDate.HasValue)
                query = query.Where(x => x.LoggedAt >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(x => x.LoggedAt <= endDate.Value);
            if (technicianId.HasValue)
                query = query.Where(x => x.TechnicianId == technicianId.Value);

            var grouped = await query
                .GroupBy(x => x.TechnicianId)
                .Select(g => new
                {
                    TechnicianId = g.Key,
                    AvgScore = g.Average(x => x.Score),
                    ProjectedJobs = g.Count(),
                    Notes = string.Join("; ", g.Select(x => x.InsightType + ": " + x.InsightDetail).Distinct())
                })
                .ToListAsync();

            var forecastDate = DateTime.UtcNow.Date;
            var forecasts = grouped.Select(g => new TechnicianForecastLog
            {
                TechnicianId = g.TechnicianId,
                ForecastDate = forecastDate,
                ProjectedJobs = g.ProjectedJobs,
                ExpectedScore = g.AvgScore == 0 ? 0 : g.AvgScore,
                ForecastNotes = g.Notes
            }).ToList();

            // Save forecasts to DB
            _db.TechnicianForecastLogs.AddRange(forecasts);
            await _db.SaveChangesAsync();
            return forecasts;
        }
    }
}
