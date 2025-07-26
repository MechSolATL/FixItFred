using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVP_Core.Data.Models
{
    public enum NotificationChannelType
    {
        Email,
        SMS,
        Push
    }

    public enum NotificationTriggerType
    {
        JobAssigned,
        OnTheWay,
        EstimateAccepted,
        TechArrived,
        PostJobFeedbackRequest
    }

    public enum NotificationStatus
    {
        Pending,
        Sent,
        Failed,
        Cancelled
    }

    public class NotificationQueue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Recipient { get; set; } = string.Empty;
        [Required]
        public NotificationChannelType ChannelType { get; set; }
        [Required]
        public NotificationTriggerType TriggerType { get; set; }
        public int TargetId { get; set; }
        [Required]
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        public DateTime ScheduledTime { get; set; }
        public DateTime? SentTime { get; set; }
        [MaxLength(2000)]
        public string MessageBody { get; set; } = string.Empty;
    }
}
