using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class TechnicianHeatmapLog
    {
        /// <summary>
        /// The unique identifier for the technician heatmap log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the heatmap log entry.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The latitude of the technician's location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude of the technician's location.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// The timestamp when the heatmap log entry was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The type of service associated with the heatmap log entry.
        /// </summary>
        public string ServiceType { get; set; } = string.Empty;

        /// <summary>
        /// The zone tag associated with the heatmap log entry.
        /// </summary>
        public string ZoneTag { get; set; } = string.Empty;
    }
}
