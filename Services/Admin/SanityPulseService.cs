using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Data.Models;
using Data;

namespace Services.Admin
{
    public class SanityPulseService
    {
        private readonly ApplicationDbContext _db;

        public SanityPulseService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task SubmitPulseAsync(WellBeingPulseLog pulse)
        {
            pulse.Timestamp = DateTime.UtcNow;
            pulse.NeedsFollowUp = pulse.MoodScore <= 5 || pulse.WorkSatisfactionScore <= 5 || (int)pulse.StressLevel >= 7;
            _db.WellBeingPulseLogs.Add(pulse);
            await _db.SaveChangesAsync();
        }

        public async Task<List<WellBeingPulseLog>> GetRecentLogsAsync(int count = 50)
        {
            return await _db.WellBeingPulseLogs
                .OrderByDescending(x => x.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<WellBeingPulseLog>> GetUnresolvedAsync()
        {
            return await _db.WellBeingPulseLogs
                .Where(x => x.NeedsFollowUp && !x.ManagerReviewed)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }

        public async Task FlagFollowUpAsync(int id)
        {
            var log = await _db.WellBeingPulseLogs.FindAsync(id);
            if (log != null)
            {
                log.NeedsFollowUp = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task ResolveLogAsync(int id, string? managerNote = null)
        {
            var log = await _db.WellBeingPulseLogs.FindAsync(id);
            if (log != null)
            {
                log.ManagerReviewed = true;
                log.ResponseDate = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(managerNote))
                    log.OpenNote = managerNote;
                await _db.SaveChangesAsync();
            }
        }

        public WellBeingPulseLog AcceptPulseEntry(string employeeId, int moodScore, int stressLevel, int workSatisfactionScore, string? note)
        {
            var pulse = new WellBeingPulseLog
            {
                EmployeeId = employeeId,
                Timestamp = DateTime.UtcNow,
                MoodScore = moodScore,
                StressLevel = (StressLevel)stressLevel,
                WorkSatisfactionScore = workSatisfactionScore,
                OpenNote = note,
                NeedsFollowUp = stressLevel >= (int)StressLevel.Level7,
                ManagerReviewed = false
            };
            _db.WellBeingPulseLogs.Add(pulse);
            _db.SaveChanges();
            return pulse;
        }

        public string GetActionableReview(WellBeingPulseLog pulse)
        {
            if ((int)pulse.StressLevel >= 7)
                return "High stress detected. Recommend manager review and follow-up.";
            if (pulse.MoodScore <= 3)
                return "Low mood. Consider support or check-in.";
            return "Pulse normal.";
        }
    }
}
