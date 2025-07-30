using System;
using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class IncidentCompressionLog
    {
        /// <summary>
        /// The unique identifier for the incident compression log.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The timestamp when the incident compression log was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The key used to cluster incidents (e.g., address, unit, complaint type).
        /// </summary>
        public string ClusterKey { get; set; } = string.Empty;

        /// <summary>
        /// The number of occurrences in the cluster.
        /// </summary>
        public int OccurrenceCount { get; set; }

        /// <summary>
        /// Details about equipment faults in the cluster, if applicable.
        /// </summary>
        public string? EquipmentFaults { get; set; }

        /// <summary>
        /// Details about dispatch issues in the cluster, if applicable.
        /// </summary>
        public string? DispatchIssues { get; set; }

        /// <summary>
        /// Indicates whether technician burnout is suspected in the cluster.
        /// </summary>
        public bool TechBurnoutSuspected { get; set; }

        /// <summary>
        /// Indicates whether client abuse is suspected in the cluster.
        /// </summary>
        public bool ClientAbuseSuspected { get; set; }

        /// <summary>
        /// Additional notes about the incident compression log.
        /// </summary>
        public string? Notes { get; set; }
    }
}
