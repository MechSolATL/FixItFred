// Sprint83.4-FinalFixSweep2
namespace MVP_Core.Data.Models
{
    public class DispatcherNotification
    {
        public int Id { get; set; } // Sprint83.4-FinalFixSweep2
        public string SentBy { get; set; } = string.Empty; // Sprint83.4-FinalFixSweep2
        public DateTime SentAt { get; set; } = DateTime.UtcNow; // Sprint83.4-FinalFixSweep2
        public string Type { get; set; } = string.Empty; // Sprint83.4-FinalFixSweep2
        public string Message { get; set; } = string.Empty; // Sprint83.4-FinalFixSweep2
    }
}