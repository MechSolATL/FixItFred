using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MVP_Core.Data.Models
{
    public class SkillTrack
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g. Safety, Technical, SoftSkill
        public bool IsRequired { get; set; } = false;
        public List<int> AssignedTo { get; set; } = new(); // Technician IDs
    }
}