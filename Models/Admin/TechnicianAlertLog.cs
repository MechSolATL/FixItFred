using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace MVP_Core.Data.Models
{
    // Sprint 84.9 — Drop Alert Logic + TrustScore Delta Detection
    // Sprint 85.3 — Triggered Coaching Suggestions
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
    }
}
