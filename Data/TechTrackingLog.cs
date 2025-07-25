// Sprint 30E - Secure Technician GPS API
using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data
{
    public class TechTrackingLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime Timestamp { get; set; }
        public string IP { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? UserAgent { get; set; }
    }
}
