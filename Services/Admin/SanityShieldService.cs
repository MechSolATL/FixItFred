using MVP_Core.Data.Models;
using MVP_Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public class SanityShieldService
    {
        private readonly ApplicationDbContext _db;
        public SanityShieldService(ApplicationDbContext db)
        {
            _db = db;
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
            // Example logic: error streaks, rapid login/logout, conflicting dispatches, escalations, roast impact
            var logs = await _db.TechnicianSanityLogs.Where(s => s.TechnicianId == technicianId).OrderByDescending(s => s.Timestamp).ToListAsync();
            var recentLog = logs.FirstOrDefault();
            bool shieldRoast = false;
            bool softenEscalation = false;
            bool alertSupervisor = false;
            string notes = "";

            // Dummy logic for illustration
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

            // Log shielding decision
            if (recentLog != null)
            {
                recentLog.ShieldingNotes = notes;
                _db.TechnicianSanityLogs.Update(recentLog);
                await _db.SaveChangesAsync();
            }
        }
    }
}
