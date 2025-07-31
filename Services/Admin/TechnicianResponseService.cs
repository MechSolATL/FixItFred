using Microsoft.EntityFrameworkCore;
using Data;
using Data.ViewModels;
using Data.Models;

namespace Services.Admin
{
    public class TechnicianResponseService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianResponseService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task LogResponseAsync(int technicianId, int requestId, string responseSource, DateTime timeSent, DateTime? timeAcknowledged, bool flaggedLate)
        {
            var log = new TechnicianResponseLog
            {
                TechnicianId = technicianId,
                RequestId = requestId,
                ResponseSource = responseSource,
                TimeSent = timeSent,
                TimeAcknowledged = timeAcknowledged,
                FlaggedLate = flaggedLate,
                LoggedAt = DateTime.UtcNow
            };
            _db.TechnicianResponseLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task<List<TechnicianResponseLog>> GetRecentResponsesAsync(DateTime? start = null, DateTime? end = null, string? source = null)
        {
            var query = _db.TechnicianResponseLogs.AsQueryable();
            if (start.HasValue)
                query = query.Where(r => r.LoggedAt >= start.Value);
            if (end.HasValue)
                query = query.Where(r => r.LoggedAt <= end.Value);
            if (!string.IsNullOrEmpty(source))
                query = query.Where(r => r.ResponseSource == source);
            return await query.OrderByDescending(r => r.LoggedAt).ToListAsync();
        }

        public async Task<List<ResponseScorecard>> GenerateResponseScorecardsAsync(DateTime? start = null, DateTime? end = null, string? source = null)
        {
            var query = _db.TechnicianResponseLogs.AsQueryable();
            if (start.HasValue)
                query = query.Where(r => r.LoggedAt >= start.Value);
            if (end.HasValue)
                query = query.Where(r => r.LoggedAt <= end.Value);
            if (!string.IsNullOrEmpty(source))
                query = query.Where(r => r.ResponseSource == source);
            var grouped = await query.GroupBy(r => r.TechnicianId)
                .Select(g => new
                {
                    TechnicianId = g.Key,
                    AverageResponseSeconds = g.Where(x => x.TimeAcknowledged.HasValue).Any() ? (int)g.Where(x => x.TimeAcknowledged.HasValue).Average(x => (x.TimeAcknowledged.Value - x.TimeSent).TotalSeconds) : 0,
                    TotalResponses = g.Count(),
                    LateCount = g.Count(x => x.FlaggedLate)
                }).ToListAsync();
            var techs = _db.Technicians.ToList();
            var scorecards = grouped.Select((g, i) => new ResponseScorecard
            {
                TechnicianName = techs.FirstOrDefault(t => t.Id == g.TechnicianId)?.FullName ?? $"Tech #{g.TechnicianId}",
                AverageResponseSeconds = g.AverageResponseSeconds,
                TotalResponses = g.TotalResponses,
                LateCount = g.LateCount,
                Rank = i + 1
            }).OrderBy(x => x.AverageResponseSeconds).ToList();
            // Re-rank by sorted avg response
            for (int i = 0; i < scorecards.Count; i++)
                scorecards[i].Rank = i + 1;
            return scorecards;
        }
    }
}
