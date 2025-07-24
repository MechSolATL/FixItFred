using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public class UserResponse
    {
        [Key]
        public int Id { get; set; }

        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; } = null!;

        public int? ServiceRequestId { get; set; }

        [ForeignKey("ServiceRequestId")]
        public ServiceRequest? ServiceRequest { get; set; }

        [Required]
        public string Response { get; set; } = string.Empty;

        [Required]
        public string ServiceType { get; set; } = string.Empty;

        [Required]
        public string SessionID { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
