using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class RoutingOverlayRegion
    {
        public int Id { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public string GeoBoundaryJson { get; set; } = string.Empty; // GeoJSON or similar
        public string PreferredTechIds { get; set; } = string.Empty; // Comma-separated IDs
        public int ZonePriority { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
