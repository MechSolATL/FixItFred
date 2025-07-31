using System;

namespace Models.Admin
{
    // Sprint 41 - Historical SLA Trend DTO
    public class SlaTrendDto
    {
        public string GroupKey { get; set; } = string.Empty;
        public string GroupType { get; set; } = string.Empty;
        public double AverageSlaMinutes { get; set; }
        public int Count { get; set; }
    }
}
