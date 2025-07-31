using System;

namespace Data.Models
{
    public class SkillBadge
    {
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public DateTime AwardedAt { get; set; }
    }
}