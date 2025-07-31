// Sprint 83.7-Hardening: Removed legacy RoastStatus reference
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class RoastDeliveryLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int RoastTemplateId { get; set; }
        public string TriggeredBy { get; set; } = string.Empty;
        public string DeliveryResult { get; set; } = string.Empty;
    }
}
