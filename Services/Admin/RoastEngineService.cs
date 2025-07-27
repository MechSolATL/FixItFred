using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MVP_Core.Services.Admin
{
    public class RoastEngineService
    {
        private readonly ApplicationDbContext _db;
        public RoastEngineService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Schedule a roast for a new hire (tiered)
        public async Task ScheduleRoastAsync(string employeeId, RoastTier tier)
        {
            var hireDate = await _db.EmployeeMilestoneLogs
                .Where(x => x.EmployeeId == employeeId && x.MilestoneType == "Hire")
                .OrderByDescending(x => x.DateRecognized)
                .Select(x => x.DateRecognized)
                .FirstOrDefaultAsync();
            if (hireDate == default) return;
            if ((DateTime.UtcNow - hireDate).TotalDays > 365) return;
            var roast = await _db.RoastTemplates
                .Where(x => x.Tier == tier && x.TimesUsed < x.UseLimit)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync();
            if (roast == null) return;
            roast.TimesUsed++;
            var log = new NewHireRoastLog
            {
                EmployeeId = employeeId,
                RoastMessage = roast.Message,
                ScheduledFor = DateTime.UtcNow.AddDays(7),
                IsDelivered = false,
                RoastLevel = (int)tier
            };
            _db.NewHireRoastLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        // Get pending roasts
        public async Task<List<NewHireRoastLog>> GetPendingRoastsAsync()
        {
            return await _db.NewHireRoastLogs
                .Where(x => !x.IsDelivered)
                .OrderBy(x => x.ScheduledFor)
                .ToListAsync();
        }

        // Deliver a roast
        public async Task DeliverRoastAsync(int roastId)
        {
            var log = await _db.NewHireRoastLogs.FindAsync(roastId);
            if (log == null || log.IsDelivered) return;
            log.IsDelivered = true;
            log.DeliveredAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        // Drop weekly roasts for new hires (tiered)
        public async Task DropWeeklyRoastsAsync()
        {
            var now = DateTime.UtcNow;
            var oneYearAgo = now.AddMonths(-12);
            var recentHires = await _db.EmployeeMilestoneLogs
                .Where(x => x.MilestoneType == "Hire" && x.DateRecognized > oneYearAgo)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToListAsync();
            var templates = await _db.RoastTemplates.Where(x => x.TimesUsed < x.UseLimit).ToListAsync();
            if (!templates.Any() || !recentHires.Any()) return;
            var rng = new Random();
            foreach (var empId in recentHires)
            {
                // Rotate tier: Soft for new, escalate by tenure
                var hireDate = await _db.EmployeeMilestoneLogs
                    .Where(x => x.EmployeeId == empId && x.MilestoneType == "Hire")
                    .OrderByDescending(x => x.DateRecognized)
                    .Select(x => x.DateRecognized)
                    .FirstOrDefaultAsync();
                var months = hireDate == default ? 0 : (now - hireDate).Days / 30;
                RoastTier tier = months switch
                {
                    < 3 => RoastTier.Soft,
                    < 6 => RoastTier.Medium,
                    < 9 => RoastTier.Savage,
                    _ => RoastTier.Brutal
                };
                var eligibleRoasts = templates.Where(x => x.Tier == tier).ToList();
                if (!eligibleRoasts.Any()) continue;
                var roast = eligibleRoasts[rng.Next(eligibleRoasts.Count)];
                roast.TimesUsed++;
                _db.NewHireRoastLogs.Add(new NewHireRoastLog
                {
                    EmployeeId = empId,
                    RoastMessage = roast.Message,
                    ScheduledFor = now,
                    IsDelivered = false,
                    RoastLevel = (int)tier
                });
                Console.WriteLine($"[RoastScheduler] {tier} roast dropped for EmployeeId={empId}: {roast.Message}");
            }
            await _db.SaveChangesAsync();
        }

        // RoastRank Fusion Suite
        public async Task<Dictionary<string, double>> GetRoastRankScoresAsync()
        {
            // Get all employees
            var employees = await _db.EmployeeMilestoneLogs.Where(x => x.MilestoneType == "Hire").Select(x => x.EmployeeId).Distinct().ToListAsync();
            var scores = new Dictionary<string, double>();
            foreach (var empId in employees)
            {
                // Find TechnicianId for this EmployeeId
                var tech = await _db.Technicians.FirstOrDefaultAsync(t => t.FullName == empId || t.Email == empId);
                if (tech == null) { scores[empId] = 0; continue; }
                int techId = tech.Id;
                // Karma
                var karma = await _db.TechnicianKarmaLogs.Where(x => x.TechnicianId == techId).Select(x => x.KarmaScore).DefaultIfEmpty(0).SumAsync();
                // Trust Index
                var trust = await _db.TechnicianTrustLogs.Where(x => x.TechnicianId == techId).Select(x => x.TrustScore).DefaultIfEmpty(0).SumAsync();
                // Speed Ranking (lower is better)
                var speed = await _db.TechnicianResponseLogs.Where(x => x.TechnicianId == techId).Select(x => x.ResponseSeconds).DefaultIfEmpty(60).AverageAsync();
                // Escalation Rate
                var escalations = await _db.TechnicianEscalationLogs.Where(x => x.TechnicianId == techId).CountAsync();
                // Past Roasts Delivered
                var roasts = await _db.NewHireRoastLogs.Where(x => x.EmployeeId == empId && x.IsDelivered).CountAsync();
                // Fusion formula (weights can be tuned)
                double roastRank = (double)karma * 0.25 + (double)trust * 0.25 + (100.0 / (1 + speed)) * 0.2 + (10.0 / (1 + escalations)) * 0.15 + roasts * 0.15;
                scores[empId] = Math.Round(roastRank, 2);
            }
            return scores;
        }
    }
}
