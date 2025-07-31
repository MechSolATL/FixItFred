using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Models.Admin
{
    // Sprint 84.9 — Drop Alert Logic + TrustScore Delta Detection
    // Sprint 85.3 — Triggered Coaching Suggestions
    // Sprint 85.7 — Admin Log Hardening & Encryption
    public class TechnicianAlertLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int PreviousScore { get; set; }
        public int CurrentScore { get; set; }
        public DateTime TriggeredAt { get; set; }
        public bool Acknowledged { get; set; } = false;
        // Sprint 85.3 — Triggered Coaching Suggestions
        public bool TriggeredCoachingRecommended { get; set; } = false;
        // Sprint 85.7 — Admin Log Hardening & Encryption
        public bool IsSensitive { get; set; } = false;
        // Sprint 86.7 — For alert/job association
        public int? ServiceRequestId { get; set; } // Sprint 86.7 — For alert/job association
        public string? AlertType { get; set; } // Sprint 86.7 — e.g. "LateETA"
        public DateTime? CreatedAt { get; set; } // Sprint 86.7 — Alert timestamp
        public string? Message { get; set; } // Sprint 86.7 — Alert message
    }
}
