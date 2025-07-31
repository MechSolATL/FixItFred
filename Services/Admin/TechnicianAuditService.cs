using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// FixItFred: Explicitly alias TechnicianBehaviorLog to resolve ambiguity
using DomainTechnicianBehaviorLog = Models.TechnicianBehaviorLog;
using Data;

namespace Services.Admin
{
    // Sprint 86.2 — Technician Accountability Engine
    public class TechnicianAuditService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianAuditService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<DomainTechnicianBehaviorLog>> GetFlaggedLogsAsync()
        {
            return await _db.Set<DomainTechnicianBehaviorLog>().Where(l => l.Status == "Flagged").ToListAsync();
        }

        public async Task<List<DomainTechnicianBehaviorLog>> GetAllLogsAsync()
        {
            return await _db.Set<DomainTechnicianBehaviorLog>().ToListAsync();
        }

        public async Task FlagIfViolationAsync(int technicianId, int serviceRequestId, DateTime actualTime, DateTime expectedTime, double actualDistance, double allowedDistance, string? metadata)
        {
            // Time mismatch > 3hrs
            if (Math.Abs((actualTime - expectedTime).TotalHours) > 3)
            {
                await AddFlagAsync(technicianId, serviceRequestId, "Time", $"Time mismatch: {actualTime} vs {expectedTime}");
            }
            // Location mismatch > 1.5 miles
            if (actualDistance > allowedDistance)
            {
                await AddFlagAsync(technicianId, serviceRequestId, "Location", $"Location mismatch: {actualDistance:F2} miles");
            }
            // Missing or mismatched metadata
            if (string.IsNullOrWhiteSpace(metadata))
            {
                await AddFlagAsync(technicianId, serviceRequestId, "Metadata", "Missing or mismatched metadata");
            }
        }

        public async Task AddFlagAsync(int technicianId, int serviceRequestId, string violationType, string notes)
        {
            var log = new DomainTechnicianBehaviorLog
            {
                TechnicianId = technicianId,
                ServiceRequestId = serviceRequestId,
                ViolationType = violationType,
                Notes = notes,
                Timestamp = DateTime.UtcNow,
                Status = "Flagged"
            };
            _db.Add(log);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateLogStatusAsync(int logId, string status, string? notes = null)
        {
            var log = await _db.Set<DomainTechnicianBehaviorLog>().FindAsync(logId);
            if (log != null)
            {
                log.Status = status;
                if (!string.IsNullOrWhiteSpace(notes))
                    log.Notes += $"\n[Admin Note] {notes}";
                await _db.SaveChangesAsync();
            }
        }

        // --- Sprint 86.2: Async stubs for TechAudit.cshtml.cs ---
        public Task<List<DomainTechnicianBehaviorLog>> GetLogsByTechAndDateAsync(int technicianId, DateTime date)
        {
            return Task.FromResult(new List<DomainTechnicianBehaviorLog>());
        }
        public Task<List<DomainTechnicianBehaviorLog>> GetLogsByRangeAsync(DateTime start, DateTime end, int? technicianId = null)
        {
            return Task.FromResult(new List<DomainTechnicianBehaviorLog>());
        }
        public Task<List<DomainTechnicianBehaviorLog>> GetLogsByActionTypeAsync(string violationType)
        {
            return Task.FromResult(new List<DomainTechnicianBehaviorLog>());
        }
    }
}
