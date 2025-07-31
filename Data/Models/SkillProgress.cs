using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class SkillProgress
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        [Required]
        public int SkillTrackId { get; set; }
        [Required]
        public string Status { get; set; } = "Assigned"; // Assigned, InProgress, Completed
        public DateTime? CompletedDate { get; set; }
    }
}