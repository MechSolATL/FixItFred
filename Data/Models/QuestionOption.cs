using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Data.Models
{
    public class QuestionOption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Option text is required.")]
        [MaxLength(300, ErrorMessage = "Option text cannot exceed 300 characters.")]
        public string OptionText { get; set; } = string.Empty;

        /// <summary>
        /// Optional raw value stored in the DB, different from user-facing text.
        /// </summary>
        [MaxLength(300)]
        public string? OptionValue { get; set; }

        /// <summary>
        /// Optional score or diagnostic weight — used for intelligent form analysis.
        /// </summary>
        public int? ScoreWeight { get; set; }

        /// <summary>
        /// Navigation property to the parent question.
        /// </summary>
        [JsonIgnore]
        public Question? Question { get; set; }
    }
}
