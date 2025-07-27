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

        // Schedule a roast for a new hire
        public async Task ScheduleRoastAsync(string employeeId, int roastLevel)
        {
            // Check tenure (assume EmployeeMilestoneLog or similar exists)
            var hireDate = await _db.EmployeeMilestoneLogs
                .Where(x => x.EmployeeId == employeeId && x.MilestoneType == "Hire")
                .OrderByDescending(x => x.DateRecognized)
                .Select(x => x.DateRecognized)
                .FirstOrDefaultAsync();
            if (hireDate == default) return;
            if ((DateTime.UtcNow - hireDate).TotalDays > 365) return; // Only <12 months

            // Pick a random roast template for the level
            var roast = await _db.RoastTemplates
                .Where(x => x.Level == roastLevel)
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync();
            if (roast == null) return;

            var log = new NewHireRoastLog
            {
                EmployeeId = employeeId,
                RoastMessage = roast.Message,
                ScheduledFor = DateTime.UtcNow.AddDays(7), // Next week
                IsDelivered = false,
                RoastLevel = roastLevel
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

        // Drop weekly roasts for new hires (Sprint 73.3)
        public async Task DropWeeklyRoastsAsync()
        {
            var now = DateTime.UtcNow;
            var oneYearAgo = now.AddMonths(-12);
            // Find AdminUsers hired in last 12 months
            var recentHires = await _db.EmployeeMilestoneLogs
                .Where(x => x.MilestoneType == "Hire" && x.DateRecognized > oneYearAgo)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToListAsync();
            var templates = await _db.RoastTemplates.ToListAsync();
            if (!templates.Any() || !recentHires.Any()) return;
            var rng = new Random();
            foreach (var empId in recentHires)
            {
                var roast = templates[rng.Next(templates.Count)];
                _db.NewHireRoastLogs.Add(new NewHireRoastLog
                {
                    EmployeeId = empId,
                    RoastMessage = roast.Message,
                    ScheduledFor = now,
                    IsDelivered = false,
                    RoastLevel = roast.Level
                });
                // Mock notification (console/log)
                Console.WriteLine($"[RoastScheduler] Roast dropped for EmployeeId={empId}: {roast.Message}");
            }
            await _db.SaveChangesAsync();
        }
    }
}
