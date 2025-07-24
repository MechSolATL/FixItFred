namespace MVP_Core.Models.Admin
{
    public class WatchdogAlert
    {
        public int RequestId { get; set; }
        public string AlertType { get; set; } // Inactivity, ETAOverdue, Unassigned
        public string Message { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
