using MVP_Core.Data.Models;
using MVP_Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    // Sprint 81: Null safety hardening for SanityShieldService.cs
    public class SanityShieldService
    {
        private readonly ApplicationDbContext _db;
        public SanityShieldService(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<List<TechnicianSanityLog>> GetSanityLogsAsync(int technicianId)
        {
            return await _db.TechnicianSanityLogs
                .Where(s => s.TechnicianId == technicianId)
                .OrderByDescending(s => s.Timestamp)
                .ToListAsync();
        }

        public async Task AnalyzeAndShieldAsync(int technicianId)
        {
            var logs = await _db.TechnicianSanityLogs.Where(s => s.TechnicianId == technicianId).OrderByDescending(s => s.Timestamp).ToListAsync();
            var recentLog = logs.FirstOrDefault();
            bool shieldRoast = false;
            bool softenEscalation = false;
            bool alertSupervisor = false;
            string notes = string.Empty;

            if (recentLog != null && recentLog.EmotionalFatigueFlag)
            {
                shieldRoast = true;
                notes += "Temporary roast skip applied. ";
            }
            if (recentLog != null && recentLog.ErrorRateSpike > 5)
            {
                softenEscalation = true;
                notes += "Escalation routing softened. ";
            }
            if (recentLog != null && recentLog.BurnoutPatternDetected)
            {
                alertSupervisor = true;
                notes += "Alert sent to dispatcher supervisor. ";
            }

            if (recentLog != null)
            {
                recentLog.ShieldingNotes = notes;
                _db.TechnicianSanityLogs.Update(recentLog);
                await _db.SaveChangesAsync();
            }
        }
    }
}
