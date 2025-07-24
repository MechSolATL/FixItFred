using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class TechnicianLoadLog
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int JobId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("TechnicianId")]
        public Technician? Technician { get; set; }
    }
}
