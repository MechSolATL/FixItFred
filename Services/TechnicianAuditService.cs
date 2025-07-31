using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;

namespace Services
{
    public class TechnicianAuditService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianAuditService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task LogEventAsync(TechnicianAuditLog log)
        {
            log.Timestamp = DateTime.UtcNow;
            _db.TechnicianAuditLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task<List<TechnicianAuditLog>> GetLogsByTechAndDateAsync(int technicianId, DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);
            return await _db.TechnicianAuditLogs
                .Where(l => l.TechnicianId == technicianId && l.Timestamp >= start && l.Timestamp < end)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<List<TechnicianAuditLog>> GetLogsByRangeAsync(DateTime start, DateTime end, int? technicianId = null)
        {
            var query = _db.TechnicianAuditLogs.AsQueryable();
            if (technicianId.HasValue)
                query = query.Where(l => l.TechnicianId == technicianId.Value);
            return await query.Where(l => l.Timestamp >= start && l.Timestamp < end)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<List<TechnicianAuditLog>> GetLogsByActionTypeAsync(TechnicianAuditActionType actionType, DateTime? start = null, DateTime? end = null)
        {
            var query = _db.TechnicianAuditLogs.Where(l => l.ActionType == actionType);
            if (start.HasValue)
                query = query.Where(l => l.Timestamp >= start.Value);
            if (end.HasValue)
                query = query.Where(l => l.Timestamp < end.Value);
            return await query.OrderByDescending(l => l.Timestamp).ToListAsync();
        }

        // Stub: Link logs to SLAAnalytics
        public async Task<object> GetSLAImpactForLogAsync(int logId)
        {
            // Implement actual SLA impact logic as needed
            await Task.CompletedTask;
            return new { Impact = "Stub" };
        }

        // Stub: Link logs to DisputeManager
        public async Task<object> GetDisputeContextForLogAsync(int logId)
        {
            await Task.CompletedTask;
            return new { Dispute = "Stub" };
        }
    }
}
