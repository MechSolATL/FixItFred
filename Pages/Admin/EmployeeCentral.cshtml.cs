using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;

namespace MVP_Core.Pages.Admin
{
    public class EmployeeCentralModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<NotificationHub> _hubContext;
        public List<MVP_Core.Data.Models.Technician> TechnicianList { get; set; } = new();
        public List<ActivityLogDto> ActivityTimeline { get; set; } = new();
        public int SevereFlagCount { get; set; }
        public double AvgTrustScore { get; set; }
        public int TotalIdleMinutes { get; set; }
        public double AvgPulseScore { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ErrorMessage { get; set; }

        public EmployeeCentralModel(ApplicationDbContext db, IHubContext<NotificationHub> hubContext)
        {
            _db = db;
            _hubContext = hubContext;
        }

        public async Task OnGetAsync(int? TechId, DateTime? StartDate, DateTime? EndDate, string? LogType, string? Keyword)
        {
            try
            {
                TechnicianList = await _db.Technicians.ToListAsync();
                var logs = new List<ActivityLogDto>();
                // Aggregate logs from all relevant sources
                var idleLogs = await _db.IdleSessionMonitorLogs.ToListAsync();
                logs.AddRange(idleLogs.Select(x => new ActivityLogDto
                {
                    EmployeeName = _db.Technicians.FirstOrDefault(t => t.Id == x.TechnicianId)?.FullName ?? "Unknown",
                    LogType = "Idle",
                    Timestamp = x.IdleStartTime,
                    Description = $"Idle for {x.IdleMinutes} minutes ({x.SystemDecision})",
                    Details = $"Override: {x.OverrideReason ?? "None"}, Approver: {x.Approver ?? "N/A"}",
                    Value = x.IdleMinutes
                }));
                var clockInLogs = await _db.LateClockInLogs.ToListAsync();
                logs.AddRange(clockInLogs.Select(x => new ActivityLogDto
                {
                    EmployeeName = _db.Technicians.FirstOrDefault(t => t.Id == x.TechnicianId)?.FullName ?? "Unknown",
                    LogType = "ClockIn",
                    Timestamp = x.Date,
                    Description = $"Late clock-in ({x.Severity})",
                    Details = $"Scheduled: {x.ScheduledStart:t}, Actual: {x.ActualStart:t}, Delay: {x.DelayMinutes} min",
                    Value = x.Severity == "Severe" ? 2 : 1
                }));
                var breakLogs = await _db.BreakComplianceOverrideLogs.ToListAsync();
                logs.AddRange(breakLogs.Select(x => new ActivityLogDto
                {
                    EmployeeName = _db.Technicians.FirstOrDefault(t => t.Id == x.TechnicianId)?.FullName ?? "Unknown",
                    LogType = "BreakCompliance",
                    Timestamp = x.Timestamp,
                    Description = $"Break compliance override ({(x.WasEmergency ? "Emergency" : "Standard")})",
                    Details = $"Role: {x.RoleOfApprover}, Justification: {x.Justification}",
                    Value = x.DurationExtended
                }));
                var overtimeLogs = await _db.OvertimeLockoutLogs.ToListAsync();
                logs.AddRange(overtimeLogs.Select(x => new ActivityLogDto
                {
                    EmployeeName = _db.Technicians.FirstOrDefault(t => t.Id == x.TechnicianId)?.FullName ?? "Unknown",
                    LogType = "Overtime",
                    Timestamp = x.EventTime,
                    Description = $"Overtime event ({x.EventType})",
                    Details = $"Decision: {x.SystemDecision}, Override: {x.OverrideReason ?? "None"}, Approver: {x.Approver ?? "N/A"}",
                    Value = x.IsOverride ? 2 : 1
                }));
                var geoLogs = await _db.GeoBreakValidationLogs.ToListAsync();
                logs.AddRange(geoLogs.Select(x => new ActivityLogDto
                {
                    EmployeeName = _db.Technicians.FirstOrDefault(t => t.Id == x.TechnicianId)?.FullName ?? "Unknown",
                    LogType = "Geo",
                    Timestamp = x.ValidationTime,
                    Description = $"Geo break validation ({x.LocationStatus})",
                    Details = $"Lat: {x.Latitude}, Long: {x.Longitude}, Stationary: {x.MinutesStationary} min, Decision: {x.SystemDecision}, Override: {x.OverrideReason ?? "None"}",
                    Value = x.IsOverride ? 2 : 1
                }));
                var reviewLogs = await _db.AnonymousReviewFormLogs.ToListAsync();
                logs.AddRange(reviewLogs.Select(x => new ActivityLogDto
                {
                    EmployeeName = "Manager " + x.ManagerId,
                    LogType = "Review",
                    Timestamp = x.TriggeredAt,
                    Description = $"Anonymous review: {x.ContextHash}",
                    Details = $"Resolution: {x.ResolutionStatus}, Initiators: {x.InitiatorCount}",
                    Value = x.InitiatorCount
                }));
                var confidenceLogs = await _db.EmployeeConfidenceDecayLogs.ToListAsync();
                logs.AddRange(confidenceLogs.Select(x => new ActivityLogDto
                {
                    EmployeeName = "Manager " + x.ManagerId,
                    LogType = "Confidence",
                    Timestamp = x.WeekStart,
                    Description = $"Confidence shift: {x.ConfidenceShift:F2}",
                    Details = $"Pulse: {x.PulseScore:F2}, Interaction: {x.InteractionMatrixScore:F2}, Notes: {x.Notes ?? "None"}",
                    Value = (int)x.ConfidenceShift
                }));
                // BehavioralDetectionLogs not shown in model, skip for now
                // Filtering
                if (TechId.HasValue)
                    logs = logs.Where(x => x.EmployeeName == TechnicianList.FirstOrDefault(t => t.Id == TechId.Value)?.FullName).ToList();
                if (StartDate.HasValue)
                    logs = logs.Where(x => x.Timestamp >= StartDate.Value).ToList();
                if (EndDate.HasValue)
                    logs = logs.Where(x => x.Timestamp <= EndDate.Value).ToList();
                if (!string.IsNullOrEmpty(LogType))
                    logs = logs.Where(x => x.LogType == LogType).ToList();
                if (!string.IsNullOrEmpty(Keyword))
                    logs = logs.Where(x => x.Description.Contains(Keyword, StringComparison.OrdinalIgnoreCase) || (x.Details?.Contains(Keyword, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
                ActivityTimeline = logs.OrderByDescending(x => x.Timestamp).ToList();
                SevereFlagCount = logs.Count(x => x.LogType == "ClockIn" && x.Description.Contains("Severe"));
                AvgTrustScore = 7.5; // Placeholder, integrate with trust scoring
                TotalIdleMinutes = logs.Where(x => x.LogType == "Idle").Sum(x => x.Value);
                AvgPulseScore = 8.2; // Placeholder, integrate with pulse scoring
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load employee activity data.";
            }
        }
        public async Task<IActionResult> OnGetDrilldownAsync(long timestampTicks)
        {
            var timestamp = new DateTime(timestampTicks);
            var log = ActivityTimeline.FirstOrDefault(x => x.Timestamp == timestamp);
            if (log == null)
            {
                return Content("<div class='alert alert-danger'>Log not found.</div>", "text/html");
            }
            // Find related logs (same employee, same day)
            var related = ActivityTimeline.Where(x => x.EmployeeName == log.EmployeeName && x.Timestamp.Date == log.Timestamp.Date && x.LogType == log.LogType).ToList();
            var html = $@"<div><b>Employee:</b> {log.EmployeeName}</div>
<b>Type:</b> {log.LogType}<br/>
<b>Time:</b> {log.Timestamp:g}<br/>
<b>Description:</b> {log.Description}<br/>
<b>Details:</b> {log.Details}<br/>
<hr/><b>Related Events:</b><ul>";
            foreach (var r in related)
            {
                html += $"<li>{r.Timestamp:g}: {r.Description}</li>";
            }
            html += "</ul>";
            return Content(html, "text/html");
        }
        public class ActivityLogDto
        {
            public string EmployeeName { get; set; } = string.Empty;
            public string LogType { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
            public string Description { get; set; } = string.Empty;
            public string? Details { get; set; }
            public int Value { get; set; }
        }
        public async Task SendActivityNotification(string message, string severity)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message, severity);
        }
    }
}
