using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Data;

namespace Services.Admin
{
    public class ScheduleConflictDetectionService
    {
        private readonly ApplicationDbContext _db;
        public ScheduleConflictDetectionService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<TechnicianScheduleConflictLog>> DetectConflictsAsync(int technicianId)
        {
            // Corrected property: AssignedTechnicianId
            var jobs = await _db.ServiceRequests.Where(x => x.AssignedTechnicianId == technicianId).OrderBy(x => x.ScheduledAt).ToListAsync();
            var conflicts = new List<TechnicianScheduleConflictLog>();
            for (int i = 0; i < jobs.Count - 1; i++)
            {
                var current = jobs[i];
                var next = jobs[i + 1];
                // Overlap
                if (current.EstimatedArrival.HasValue && next.ScheduledAt.HasValue && current.EstimatedArrival > next.ScheduledAt)
                {
                    conflicts.Add(new TechnicianScheduleConflictLog
                    {
                        TechnicianId = technicianId,
                        JobId = current.Id,
                        ConflictDetectedAt = DateTime.UtcNow,
                        ConflictType = "Overlap",
                        Details = $"Job {current.Id} overlaps with Job {next.Id}",
                        IsAcknowledged = false,
                        IsResolved = false
                    });
                }
                // Impossible travel window (assume 30 min travel required)
                if (current.EstimatedArrival.HasValue && next.ScheduledAt.HasValue && (next.ScheduledAt.Value - current.EstimatedArrival.Value).TotalMinutes < 30)
                {
                    conflicts.Add(new TechnicianScheduleConflictLog
                    {
                        TechnicianId = technicianId,
                        JobId = next.Id,
                        ConflictDetectedAt = DateTime.UtcNow,
                        ConflictType = "TravelWindow",
                        Details = $"Not enough travel time between Job {current.Id} and Job {next.Id}",
                        IsAcknowledged = false,
                        IsResolved = false
                    });
                }
                // Priority clash (if both jobs are marked high priority)
                if (current.Priority == "High" && next.Priority == "High")
                {
                    conflicts.Add(new TechnicianScheduleConflictLog
                    {
                        TechnicianId = technicianId,
                        JobId = next.Id,
                        ConflictDetectedAt = DateTime.UtcNow,
                        ConflictType = "PriorityClash",
                        Details = $"High priority clash between Job {current.Id} and Job {next.Id}",
                        IsAcknowledged = false,
                        IsResolved = false
                    });
                }
            }
            if (conflicts.Any())
            {
                _db.TechnicianScheduleConflictLogs.AddRange(conflicts);
                await _db.SaveChangesAsync();
            }
            return conflicts;
        }
        public async Task<List<TechnicianScheduleConflictLog>> GetConflictsAsync()
        {
            return await _db.TechnicianScheduleConflictLogs.OrderByDescending(x => x.ConflictDetectedAt).ToListAsync();
        }
        public async Task AcknowledgeConflictAsync(int conflictId, string note)
        {
            var conflict = await _db.TechnicianScheduleConflictLogs.FindAsync(conflictId);
            if (conflict != null)
            {
                conflict.IsAcknowledged = true;
                conflict.ManualOverrideNote = note;
                await _db.SaveChangesAsync();
            }
        }
        public async Task ResolveConflictAsync(int conflictId, string suggestion)
        {
            var conflict = await _db.TechnicianScheduleConflictLogs.FindAsync(conflictId);
            if (conflict != null)
            {
                conflict.IsResolved = true;
                conflict.ResolutionSuggestion = suggestion;
                await _db.SaveChangesAsync();
            }
        }
    }
}
