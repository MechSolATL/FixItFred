using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    // Sprint 45 – Technician Device Registration for Push Notifications
    public class TechnicianDeviceRegistration
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        [Required]
        public string DeviceToken { get; set; } = string.Empty;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        [Required]
        public string Platform { get; set; } = string.Empty; // e.g., "iOS", "Android"
    }
}
