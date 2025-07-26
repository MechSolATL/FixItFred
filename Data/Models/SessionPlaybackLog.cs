// Created by FixItFred for Sprint 70.5 — Session replay metadata
namespace MVP_Core.Data.Models;
public class SessionPlaybackLog
{
    public int Id { get; set; }
    public string SessionId { get; set; }
    public string RecordedBy { get; set; }
    public string EventDataJson { get; set; }
    public DateTime RecordedAt { get; set; }
}
