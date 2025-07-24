namespace MVP_Core.Models.Admin
{
    public class DispatcherNotification
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } // Emergency, Delay, Reassigned, Info
        public string Message { get; set; }
    }
}
