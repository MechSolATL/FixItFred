using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class TechnicianSkillMap
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public int SkillId { get; set; }
        [ForeignKey("TechnicianId")]
        public Technician? Technician { get; set; }
        [ForeignKey("SkillId")]
        public TechnicianSkill? Skill { get; set; }
    }
}
