public class NewHireRoastLog
{
    public int Id { get; set; }
    public string EmployeeId { get; set; }
    public string RoastMessage { get; set; }
    public DateTime ScheduledFor { get; set; }
    public bool IsDelivered { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public int RoastLevel { get; set; } // 1 = Light, 2 = Medium, 3 = Savage
}
