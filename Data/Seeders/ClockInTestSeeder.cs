using MVP_Core.Data.Models;
using MVP_Core.Data;
using System;
using System.Linq;

namespace MVP_Core.Data.Seeders
{
    public static class ClockInTestSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            var now = DateTime.UtcNow.Date;
            if (!db.LateClockInLogs.Any())
            {
                db.LateClockInLogs.AddRange(
                    new LateClockInLog { TechnicianId = 1, ScheduledStart = now.AddHours(8), ActualStart = now.AddHours(8).AddMinutes(7), DelayMinutes = 7, Date = now, Severity = "Warning" },
                    new LateClockInLog { TechnicianId = 1, ScheduledStart = now.AddDays(-1).AddHours(8), ActualStart = now.AddDays(-1).AddHours(8).AddMinutes(18), DelayMinutes = 18, Date = now.AddDays(-1), Severity = "Moderate" },
                    new LateClockInLog { TechnicianId = 1, ScheduledStart = now.AddDays(-2).AddHours(8), ActualStart = now.AddDays(-2).AddHours(8).AddMinutes(32), DelayMinutes = 32, Date = now.AddDays(-2), Severity = "Severe" },
                    new LateClockInLog { TechnicianId = 2, ScheduledStart = now.AddHours(8), ActualStart = now.AddHours(8).AddMinutes(5), DelayMinutes = 5, Date = now, Severity = "Warning" },
                    new LateClockInLog { TechnicianId = 3, ScheduledStart = now.AddHours(8), ActualStart = now.AddHours(8).AddMinutes(0), DelayMinutes = 0, Date = now, Severity = "OnTime" }
                );
                db.SaveChanges();
            }
            if (!db.EscalationEvents.Any())
            {
                db.EscalationEvents.Add(new EscalationEvent {
                    TechnicianId = 1,
                    DispatcherId = 0,
                    IncidentDate = now,
                    Reason = "3 late clock-ins in 7 days",
                    EscalatedTo = "HR/Management",
                    Status = "Open"
                });
                db.SaveChanges();
            }
        }
    }
}
