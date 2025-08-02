// [Sprint1002_FixItFred] Created missing JobMetaData model to resolve ReplayEngineService compilation error
// This model was referenced but missing from codebase
// Sprint1002: Add missing model definitions with null validation

using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class JobMetaData
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Job ID is required.")]
        [MaxLength(100)]
        public string? JobId { get; set; }

        [Required(ErrorMessage = "Job name is required.")]
        [MaxLength(200)]
        public string? JobName { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(1000)]
        public string? MetadataJson { get; set; }

        public int Priority { get; set; } = 0;

        [MaxLength(50)]
        public string? JobType { get; set; } = "General";
    }
}