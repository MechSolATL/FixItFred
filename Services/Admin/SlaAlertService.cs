using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Models;

namespace Services.Admin
{
    public class SlaAlertService
    {
        private readonly ApplicationDbContext _db;
        public SlaAlertService(ApplicationDbContext db)
        {
            _db = db;
        }

        public ApplicationDbContext Db => _db;

        public async Task CheckSlaViolationAsync(int technicianId, DateTime scheduledArrival, DateTime actualArrival, int serviceRequestId, int slaMinutes)
        {
            var violationMinutes = (int)(actualArrival - scheduledArrival).TotalMinutes;
            bool violated = violationMinutes > slaMinutes;
            if (violated)
            {
                var alert = new SlaMissAlert
                {
                    TechnicianId = technicianId,
                    ScheduledArrival = scheduledArrival,
                    ActualArrival = actualArrival,
                    ServiceRequestId = serviceRequestId,
                    SlaViolated = true,
                    ViolationMinutes = violationMinutes,
                    AutoFlagged = true,
                    AlertSentAt = DateTime.UtcNow,
                    ResolutionStatus = "Unresolved"
                };
                _db.SlaMissAlerts.Add(alert);
                await _db.SaveChangesAsync();
            }
        }

        public async Task SendSlaAlertAsync(int alertId)
        {
            var alert = await _db.SlaMissAlerts.FindAsync(alertId);
            if (alert != null && alert.AlertSentAt == null)
            {
                alert.AlertSentAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        public async Task LogViolationAsync(SlaMissAlert alert)
        {
            _db.SlaMissAlerts.Add(alert);
            await _db.SaveChangesAsync();
        }

        public async Task<List<SlaMissAlert>> GetUnresolvedViolations()
        {
            return await _db.SlaMissAlerts.Where(a => a.ResolutionStatus == "Unresolved").OrderByDescending(a => a.AlertSentAt).ToListAsync();
        }
    }
}