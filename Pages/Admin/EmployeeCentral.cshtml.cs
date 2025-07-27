using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using Newtonsoft.Json;

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
        // Chart data endpoints for AJAX
        public JsonResult OnGetChartData(string chartType, int? techId, DateTime? startDate, DateTime? endDate)
        {
            switch (chartType)
            {
                case "trustScore":
                    return new JsonResult(new {
                        labels = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-i).ToString("MM-dd")).Reverse().ToList(),
                        data = new List<double> { 7.2, 7.5, 7.1, 7.8, 7.6, 7.4, 7.7 }
                    });
                case "idleMinutes":
                    return new JsonResult(new {
                        labels = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-i).ToString("MM-dd")).Reverse().ToList(),
                        data = new List<int> { 12, 8, 15, 10, 9, 14, 11 }
                    });
                case "geoBreak":
                    return new JsonResult(new {
                        labels = new List<string> { "Valid", "Invalid", "Override" },
                        data = new List<int> { 18, 5, 3 }
                    });
                case "overtime":
                    return new JsonResult(new {
                        labels = new List<string> { "Standard", "Emergency", "Override" },
                        data = new List<int> { 10, 4, 2 }
                    });
                case "confidenceGeo":
                    return new JsonResult(new {
                        labels = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-i).ToString("MM-dd")).Reverse().ToList(),
                        confidence = new List<double> { 8.1, 7.9, 7.5, 7.2, 7.0, 6.8, 6.5 },
                        geoFlags = new List<int> { 1, 2, 1, 3, 2, 1, 2 }
                    });
                case "clockinTrust":
                    return new JsonResult(new {
                        labels = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-i).ToString("MM-dd")).Reverse().ToList(),
                        lateClockin = new List<int> { 2, 1, 0, 3, 1, 2, 1 },
                        trust = new List<double> { 7.2, 7.1, 7.0, 6.8, 7.0, 7.3, 7.4 }
                    });
                case "overrideEmergency":
                    return new JsonResult(new {
                        labels = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-i).ToString("MM-dd")).Reverse().ToList(),
                        @override = new List<int> { 1, 0, 2, 1, 3, 2, 1 },
                        emergency = new List<int> { 0, 1, 1, 2, 1, 0, 1 }
                    });
                default:
                    return new JsonResult(new { labels = new List<string>(), data = new List<int>() });
            }
        }
    } // End of class
} // End of namespace
