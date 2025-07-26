using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class TechnicianSkillMatrix
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Technician")]
        public int TechnicianId { get; set; }
        public string SkillTag { get; set; } = string.Empty;
        public int ProficiencyLevel { get; set; } // 1–10
        public decimal ExperienceYears { get; set; }
        public DateTime LastUpdated { get; set; }

        public Technician? Technician { get; set; }
    }
}
