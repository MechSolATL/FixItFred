using System;

namespace MVP_Core.Models.Admin
{
    // Sprint 41 - Historical SLA Trend DTO
    public class SlaTrendDto
    {
        public DateTime Period { get; set; } // Day or week start
        public int TotalJobs { get; set; }
        public int SlaMet { get; set; }
        public int SlaMissed { get; set; }
        public double SlaComplianceRate => TotalJobs > 0 ? (SlaMet / (double)TotalJobs) : 0;
        public string? Zone { get; set; } // Optional: for zone-based heatmap
    }
}
