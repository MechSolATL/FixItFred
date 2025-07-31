namespace Models.Admin
{
    public class WatchdogAlert
    {
        public int RequestId { get; set; }
        public required string AlertType { get; set; }
        public required string Message { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
