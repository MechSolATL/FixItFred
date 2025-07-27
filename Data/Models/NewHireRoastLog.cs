public class NewHireRoastLog
{
    public int Id { get; set; }
    public string EmployeeId { get; set; }
    public string RoastMessage { get; set; }
    public DateTime ScheduledFor { get; set; }
    public bool IsDelivered { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public int RoastLevel { get; set; } // 0 = Soft, 1 = Medium, 2 = Savage, 3 = Brutal
    // Sprint 73.8: Karma Feedback Loop + Roast Responder Metrics
    public int ReactionScore { get; set; } // Aggregate or average score
    public string? ResponseNote { get; set; } // Nullable feedback note
    public DateTime? ReceivedAt { get; set; } // Timestamp when feedback received
}
