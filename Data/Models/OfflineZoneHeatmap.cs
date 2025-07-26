using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class OfflineZoneHeatmap
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public string ZipCode { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int FailureCount { get; set; }
        public DateTime LastFailureAt { get; set; }
        public string? Notes { get; set; }
    }
}
