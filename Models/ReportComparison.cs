// [Sprint1002_FixItFred] Created missing ReportComparison model to resolve TechnicianReportService compilation error
// This model was referenced but missing from codebase
// Sprint1002: Add missing model definitions with null validation

using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ReportComparison
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Report A ID is required.")]
        public int ReportAId { get; set; }

        [Required(ErrorMessage = "Report B ID is required.")]
        public int ReportBId { get; set; }

        [MaxLength(200)]
        public string? ComparisonType { get; set; } = "Performance";

        public DateTime ComparedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Results { get; set; }

        [MaxLength(100)]
        public string? Initiator { get; set; }

        [Range(0, 100)]
        public double SimilarityScore { get; set; } = 0.0;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}