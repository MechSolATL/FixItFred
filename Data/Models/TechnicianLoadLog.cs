using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class TechnicianLoadLog
    {
        /// <summary>
        /// The unique identifier for the technician load log entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the technician associated with the load log entry.
        /// </summary>
        public int TechnicianId { get; set; }

        /// <summary>
        /// The ID of the job assigned to the technician.
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// The timestamp when the job was assigned to the technician.
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The technician associated with the load log entry.
        /// </summary>
        [ForeignKey("TechnicianId")]
        public Technician? Technician { get; set; }
    }
}
