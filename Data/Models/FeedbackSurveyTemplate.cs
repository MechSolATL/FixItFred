using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class FeedbackSurveyTemplate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string QuestionText { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)]
        public string InputType { get; set; } = string.Empty; // slider, checkbox, text
        public int SortOrder { get; set; }
    }

    public class FeedbackResponse
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("FeedbackSurveyTemplate")]
        public int SurveyTemplateId { get; set; }
        public FeedbackSurveyTemplate? SurveyTemplate { get; set; }
        [Required]
        [MaxLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string ResponseValue { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [ForeignKey("ServiceRequest")]
        public int ServiceRequestId { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }
    }
}
