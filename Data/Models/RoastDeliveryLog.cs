// Sprint 83.6-RoastRoulette
using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    // Sprint 83.6-RoastRoulette
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
