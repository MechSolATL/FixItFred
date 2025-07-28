using MVP_Core.Data;
using MVP_Core.Data.Models;
using MVP_Core.Services; // For INotificationService
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    // Sprint 91.7.8.2: Manager action execution, audit, and notification
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

        // Cancel a request with audit and notification
        public async Task<bool> CancelRequestAsync(int requestId, string reason, string manager, INotificationService notificationService, AuditLogger auditLogger, string ip)
        {
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == requestId);
            if (req == null) return false;
            var oldStatus = req.Status;
            req.Status = "Cancelled";
            req.Notes = $"[Manager Cancelled] {reason}\n" + (req.Notes ?? "");
            await _db.SaveChangesAsync();
            // Audit
            var log = auditLogger.CreateEncryptedLog("CancelRequest", 0, oldStatus, req.Status, ip, "Standard");
            _db.AuditLogs.Add(log);
            await _db.SaveChangesAsync();
            // Notify
            await notificationService.SendAsync(req.Email, $"Your request #{req.Id} was cancelled by a manager. Reason: {reason}");
            // Optionally: SignalR broadcast here
            return true;
        }

        // Reassign technician with audit and notification
        public async Task<bool> ReassignTechnicianAsync(int requestId, int newTechId, string manager, INotificationService notificationService, AuditLogger auditLogger, string ip)
        {
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == requestId);
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == newTechId);
            if (req == null || tech == null) return false;
            var oldTech = req.AssignedTechnicianId;
            req.AssignedTechnicianId = tech.Id;
            req.AssignedTo = tech.FullName;
            req.Status = "Assigned";
            await _db.SaveChangesAsync();
            // Audit
            var log = auditLogger.CreateEncryptedLog("ReassignTechnician", 0, oldTech?.ToString() ?? "", tech.Id.ToString(), ip, "Standard");
            _db.AuditLogs.Add(log);
            await _db.SaveChangesAsync();
            // Notify
            await notificationService.SendAsync(tech.Email ?? "", $"You have been assigned to request #{req.Id} by a manager.");
            // Optionally: SignalR broadcast here
            return true;
        }

        // Reopen a closed ticket with audit and notification
        public async Task<bool> ReopenTicketAsync(int requestId, string managerNotes, string manager, INotificationService notificationService, AuditLogger auditLogger, string ip)
        {
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == requestId);
            if (req == null || req.Status == "Open" || req.Status == "Pending") return false;
            var oldStatus = req.Status;
            req.Status = "Pending";
            req.ClosedAt = null;
            req.Notes = $"[Manager Reopened] {managerNotes}\n" + (req.Notes ?? "");
            await _db.SaveChangesAsync();
            // Audit
            var log = auditLogger.CreateEncryptedLog("ReopenTicket", 0, oldStatus, req.Status, ip, "Standard");
            _db.AuditLogs.Add(log);
            await _db.SaveChangesAsync();
            // Notify
            await notificationService.SendAsync(req.Email, $"Your request #{req.Id} was reopened by a manager. Notes: {managerNotes}");
            // Optionally: SignalR broadcast here
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
