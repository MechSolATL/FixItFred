using System.Collections.Generic;

namespace MVP_Core.Data.DTO.Analytics
{
    public class ZoneAlertHeatmapDto
    {
        public List<ZoneAlert> Zones { get; set; } = new();
    }
    public class ZoneAlert
    {
        public string ZoneName { get; set; } = string.Empty;
        public int AlertCount { get; set; }
        public double Density { get; set; }
    }
}
