using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class TechnicianActivityFeedLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        [Required]
        public string ActivityType { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public string MetaDataJson { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;
        public string SessionId { get; set; } = string.Empty;
        public string RouteGroupTag { get; set; } = string.Empty;
    }
}
