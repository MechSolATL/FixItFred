using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class TechnicianHeatmapLog
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string ZoneTag { get; set; } = string.Empty;
    }
}
