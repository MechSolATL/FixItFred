using MVP_Core.Data;
using MVP_Core.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    // Sprint 91.7.8.1: Core logic for manager interventions
    public class ManagerInterventionService
    {
        private readonly ApplicationDbContext _db;
        public ManagerInterventionService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Cancel a technician assignment for a request
        public async Task<bool> CancelTechnicianAssignment(int requestId)
        {
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == requestId);
            if (req == null) return false;
            req.AssignedTechnicianId = null;
            req.AssignedTo = null;
            req.Status = "Pending";
            await _db.SaveChangesAsync();
            return true;
        }

        // Force assign a technician to a request
        public async Task<bool> ForceAssignTechnician(int requestId, int technicianId)
        {
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == requestId);
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == technicianId);
            if (req == null || tech == null) return false;
            req.AssignedTechnicianId = tech.Id;
            req.AssignedTo = tech.FullName;
            req.Status = "Assigned";
            await _db.SaveChangesAsync();
            return true;
        }

        // Reset a technician's route (clear all their active jobs)
        public async Task<bool> ResetRoute(int technicianId)
        {
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == technicianId);
            if (tech == null) return false;
            var jobs = _db.ScheduleQueues.Where(q => q.TechnicianId == tech.Id && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched)).ToList();
            foreach (var job in jobs)
            {
                job.TechnicianId = 0;
                job.AssignedTechnicianName = string.Empty;
                job.TechnicianStatus = "Unassigned";
                job.Status = ScheduleStatus.Pending;
            }
            await _db.SaveChangesAsync();
            return true;
        }

        // Reopen a closed request
        public async Task<bool> ReopenRequest(int requestId)
        {
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == requestId);
            if (req == null || req.Status == "Open" || req.Status == "Pending") return false;
            req.Status = "Pending";
            req.ClosedAt = null;
            await _db.SaveChangesAsync();
            return true;
        }

        // Flag a technician for review
        public async Task<bool> FlagTechnician(int technicianId, string reason)
        {
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == technicianId);
            if (tech == null) return false;
            _db.FlagLogs.Add(new FlagLog
            {
                // Add fields as needed, e.g. TechnicianId, Reason
            });
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
