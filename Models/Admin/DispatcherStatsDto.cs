namespace MVP_Core.Models.Admin
{
    public class DispatcherStatsDto
    {
        public int TotalActiveRequests { get; set; }
        public int TechsInTransit { get; set; }
        public int FollowUps { get; set; }
        public int Delays { get; set; }
        public string TopServiceType { get; set; }
    }
}
