// Created by FixItFred for Sprint 70.5 — Session replay metadata
namespace MVP_Core.Data.Models;
public class SessionPlaybackLog
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public string RecordedBy { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public string EventDataJson { get; set; } = string.Empty; // Sprint 78.1: Initialized to prevent CS8618 warning
    public DateTime RecordedAt { get; set; }
}
