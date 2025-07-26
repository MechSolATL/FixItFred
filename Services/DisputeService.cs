using MVP_Core.Data;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MVP_Core.Services
{
    public class DisputeService
    {
        private readonly ApplicationDbContext _db;
        public DisputeService(ApplicationDbContext db)
        {
            _db = db;
        }
        public void SubmitDispute(DisputeRecord dispute)
        {
            dispute.SubmittedDate = DateTime.UtcNow;
            _db.DisputeRecords.Add(dispute);
            _db.SaveChanges();
        }
        public List<DisputeRecord> GetDisputesByCustomer(string email)
        {
            return _db.DisputeRecords.Where(d => d.CustomerEmail == email).ToList();
        }
        public List<DisputeRecord> GetDisputesByServiceRequest(int serviceRequestId)
        {
            return _db.DisputeRecords.Where(d => d.ServiceRequestId == serviceRequestId).ToList();
        }
        public List<DisputeRecord> GetAllDisputes()
        {
            return _db.DisputeRecords.ToList();
        }
        public List<DisputeRecord> FilterDisputes(string? status, string? reason, int? escalationLevel)
        {
            var query = _db.DisputeRecords.AsQueryable();
            if (!string.IsNullOrEmpty(status)) query = query.Where(d => d.Status == status);
            if (!string.IsNullOrEmpty(reason)) query = query.Where(d => d.Reason == reason);
            if (escalationLevel.HasValue) query = query.Where(d => d.EscalationLevel == escalationLevel.Value);
            return query.ToList();
        }
        public void UpdateDisputeStatus(int id, string status, string resolutionNotes, string reviewedBy, int escalationLevel)
        {
            var dispute = _db.DisputeRecords.FirstOrDefault(d => d.Id == id);
            if (dispute != null)
            {
                dispute.Status = status;
                dispute.ResolutionNotes = resolutionNotes;
                dispute.ReviewedBy = reviewedBy;
                dispute.EscalationLevel = escalationLevel;
                _db.SaveChanges();
            }
        }
    }
}
