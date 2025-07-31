using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class TechnicianSkill
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int? Level { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
