// FixItFred — Sprint 44 Final Model Recovery
public class TechnicianMessage {
    public int MessageId { get; set; }
    public int TechnicianId { get; set; }
    public int RequestId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool ReadFlag { get; set; }
}