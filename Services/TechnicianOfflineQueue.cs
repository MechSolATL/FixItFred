using System;
using System.Collections.Generic;
using Data.Models;

namespace Services
{
    /// <summary>
    /// TechnicianOfflineQueue: Stores pending compliance checks for offline mode.
    /// </summary>
    public class TechnicianOfflineQueue
    {
        public class PendingComplianceJob
        {
            public int ServiceRequestId { get; set; }
            public int TechnicianId { get; set; }
            public DateTime QueuedAt { get; set; } = DateTime.UtcNow;
            public bool PendingCompliance { get; set; } = true;
        }
        private readonly List<PendingComplianceJob> _queue = new();
        public void AddPendingCompliance(int requestId, int technicianId)
        {
            _queue.Add(new PendingComplianceJob
            {
                ServiceRequestId = requestId,
                TechnicianId = technicianId,
                QueuedAt = DateTime.UtcNow,
                PendingCompliance = true
            });
        }
        public List<PendingComplianceJob> GetPendingComplianceJobs()
            => _queue.FindAll(j => j.PendingCompliance);
        public void MarkComplianceChecked(int requestId, int technicianId)
        {
            var job = _queue.Find(j => j.ServiceRequestId == requestId && j.TechnicianId == technicianId);
            if (job != null)
                job.PendingCompliance = false;
        }
    }
}
