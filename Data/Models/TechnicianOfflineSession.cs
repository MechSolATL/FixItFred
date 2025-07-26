using System.ComponentModel.DataAnnotations;

namespace MVP_Core.Data.Models
{
    public class TechnicianOfflineSession
    {
        [Key]
        public int Id { get; set; }
        public int TechnicianId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [MaxLength(20)]
        public string? LocationZip { get; set; }
        public string? AffectedServiceRequestId { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
