using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class ToolInventory
    {
        [Key]
        public int ToolId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? AssignedTechId { get; set; }
        [ForeignKey("AssignedTechId")]
        public Technician? AssignedTechnician { get; set; }

        [MaxLength(50)]
        public string ConditionStatus { get; set; } = "Good";

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ToolTransferLog
    {
        [Key]
        public int TransferId { get; set; }

        [Required]
        public int ToolId { get; set; }
        [ForeignKey("ToolId")]
        public ToolInventory Tool { get; set; } = null!;

        [Required]
        public int FromTechId { get; set; }
        [ForeignKey("FromTechId")]
        public Technician FromTechnician { get; set; } = null!;

        [Required]
        public int ToTechId { get; set; }
        [ForeignKey("ToTechId")]
        public Technician ToTechnician { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool ConfirmedByReceiver { get; set; } = false;
    }
}
