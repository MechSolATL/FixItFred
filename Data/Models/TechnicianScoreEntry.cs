using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    // Sprint 35 - Technician Reward Scoring System
    public class TechnicianScoreEntry
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public string Type { get; set; } = string.Empty; // e.g. "Punctuality", "SLA", "Feedback"
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
