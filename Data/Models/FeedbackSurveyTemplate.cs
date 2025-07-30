using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class FeedbackSurveyTemplate
    {
        /// <summary>
        /// The unique identifier for the feedback survey template.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The type of service associated with the survey.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ServiceType { get; set; } = string.Empty;

        /// <summary>
        /// The text of the survey question.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string QuestionText { get; set; } = string.Empty;

        /// <summary>
        /// The input type for the survey question (e.g., slider, checkbox, text).
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string InputType { get; set; } = string.Empty;

        /// <summary>
        /// The order in which the survey question appears.
        /// </summary>
        public int SortOrder { get; set; }
    }

    public class FeedbackResponse
    {
        /// <summary>
        /// The unique identifier for the feedback response.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the associated survey template.
        /// </summary>
        [ForeignKey("FeedbackSurveyTemplate")]
        public int SurveyTemplateId { get; set; }

        /// <summary>
        /// The survey template associated with the response.
        /// </summary>
        public FeedbackSurveyTemplate? SurveyTemplate { get; set; }

        /// <summary>
        /// The email address of the customer providing the feedback.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// The value of the customer's response.
        /// </summary>
        [MaxLength(2000)]
        public string ResponseValue { get; set; } = string.Empty;

        /// <summary>
        /// The timestamp when the feedback response was submitted.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The ID of the associated service request.
        /// </summary>
        [ForeignKey("ServiceRequest")]
        public int ServiceRequestId { get; set; }

        /// <summary>
        /// The service request associated with the feedback response.
        /// </summary>
        public ServiceRequest? ServiceRequest { get; set; }
    }
}
