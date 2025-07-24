using MVP_Core.Data.Models;
using MVP_Core.Models.Admin;
using MVP_Core.Models.Mobile;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public class DispatcherService
    {
        private static List<DispatcherAuditLog> _auditLog = new();
        private static List<DispatcherBroadcast> _broadcasts = new();
        private static List<TechnicianStatusDto> _techHeartbeats = new()
        {
            new TechnicianStatusDto { TechnicianId = 1, Name = "Alice Smith", LastPing = DateTime.UtcNow.AddMinutes(-5) },
            new TechnicianStatusDto { TechnicianId = 2, Name = "Bob Jones", LastPing = DateTime.UtcNow.AddMinutes(-12) },
            new TechnicianStatusDto { TechnicianId = 3, Name = "Carlos Lee", LastPing = DateTime.UtcNow.AddMinutes(-22) },
            new TechnicianStatusDto { TechnicianId = 4, Name = "Dana Patel", LastPing = DateTime.UtcNow.AddMinutes(-8) },
            new TechnicianStatusDto { TechnicianId = 5, Name = "Evan Kim", LastPing = DateTime.UtcNow.AddMinutes(-18) }
        };
        private static List<AssignmentLogEntry> _assignmentLogs = new();

        public DispatcherResult AssignTechnician(int requestId)
        {
            // Stub: validate, assign, log
            return new DispatcherResult
            {
                Message = "Technician assigned.",
                RequestDetails = $"Request {requestId} details...",
                TechnicianList = GetSuggestedTechnicians(requestId),
                ETA = EstimateArrivalTime(requestId, 1), // stub techId
                GeoLink = GetGeoLink(requestId),
                RequestSummary = $"Summary for request {requestId}",
                AssignedTechName = "Tech A"
            };
        }
        public DispatcherResult MoveUp(int requestId) => new DispatcherResult { Message = "Moved up.", ETA = EstimateArrivalTime(requestId, 1), GeoLink = GetGeoLink(requestId) };
        public DispatcherResult MoveDown(int requestId) => new DispatcherResult { Message = "Moved down.", ETA = EstimateArrivalTime(requestId, 1), GeoLink = GetGeoLink(requestId) };
        public DispatcherResult Reassign(int requestId) => new DispatcherResult { Message = "Reassigned.", ETA = EstimateArrivalTime(requestId, 1), GeoLink = GetGeoLink(requestId) };
        public DispatcherResult Cancel(int requestId) => new DispatcherResult { Message = "Cancelled.", ETA = EstimateArrivalTime(requestId, 1), GeoLink = GetGeoLink(requestId) };
        public DispatcherResult ResendInstructions(int requestId) => new DispatcherResult { Message = "Instructions resent.", ETA = EstimateArrivalTime(requestId, 1), GeoLink = GetGeoLink(requestId) };
        public DispatcherResult Escalate(int requestId) => new DispatcherResult { Message = "Escalated.", ETA = EstimateArrivalTime(requestId, 1), GeoLink = GetGeoLink(requestId) };
        public DispatcherResult ShowMap(int requestId) => new DispatcherResult { Message = "Map shown.", ETA = EstimateArrivalTime(requestId, 1), GeoLink = GetGeoLink(requestId) };

        public List<string> GetSuggestedTechnicians(int requestId)
        {
            // Stub: return top 5 by availability
            return new List<string> { "Tech A", "Tech B", "Tech C", "Tech D", "Tech E" };
        }

        public string EstimateArrivalTime(int requestId, int technicianId)
        {
            // Stub: random ETA 15-60 min
            var rand = new Random(requestId + technicianId);
            int eta = rand.Next(15, 61);
            return $"{eta} min";
        }

        public string GetGeoLink(int requestId)
        {
            // Stub: mock lat/long
            double lat = 33.7490 + (requestId % 10) * 0.01;
            double lng = -84.3880 + (requestId % 10) * 0.01;
            return $"https://maps.google.com/?q={lat},{lng}";
        }

        public List<TechnicianStatusDto> GetAllTechnicianStatuses()
        {
            return new List<TechnicianStatusDto>
            {
                new TechnicianStatusDto { TechnicianId = 1, Name = "Alice Smith", Status = "Available", AssignedJobs = 2, LastUpdate = DateTime.Now.AddMinutes(-5) },
                new TechnicianStatusDto { TechnicianId = 2, Name = "Bob Jones", Status = "On Job", AssignedJobs = 1, LastUpdate = DateTime.Now.AddMinutes(-10) },
                new TechnicianStatusDto { TechnicianId = 3, Name = "Carlos Lee", Status = "Delayed", AssignedJobs = 3, LastUpdate = DateTime.Now.AddMinutes(-20) },
                new TechnicianStatusDto { TechnicianId = 4, Name = "Dana Patel", Status = "Unavailable", AssignedJobs = 0, LastUpdate = DateTime.Now.AddMinutes(-30) },
                new TechnicianStatusDto { TechnicianId = 5, Name = "Evan Kim", Status = "Available", AssignedJobs = 1, LastUpdate = DateTime.Now.AddMinutes(-2) }
            };
        }
        public int CalculateDispatchScore(int techId)
        {
            // Factors: fewer open jobs = higher score, no callbacks = bonus, last ping < 10 min = bonus, >4 active requests = penalty, recent delays = penalty
            var tech = _techHeartbeats.FirstOrDefault(t => t.TechnicianId == techId);
            if (tech == null) return 0;
            int score = 100;
            // Mock: open jobs
            int openJobs = new Random(techId).Next(0, 7);
            score -= openJobs * 10;
            // Mock: callbacks
            int callbacks = new Random(techId + 100).Next(0, 3);
            if (callbacks == 0) score += 10;
            else score -= callbacks * 5;
            // Last ping bonus
            if ((DateTime.UtcNow - tech.LastPing).TotalMinutes < 10) score += 10;
            // Overload penalty
            if (openJobs > 4) score -= 20;
            // Recent delays penalty
            bool recentDelay = new Random(techId + 200).Next(0, 2) == 1;
            if (recentDelay) score -= 10;
            return Math.Max(0, Math.Min(100, score));
        }
        public List<TechnicianStatusDto> GetAllTechnicianHeartbeats()
        {
            foreach (var tech in _techHeartbeats)
            {
                tech.DispatchScore = CalculateDispatchScore(tech.TechnicianId);
            }
            return _techHeartbeats.OrderByDescending(t => t.DispatchScore).ToList();
        }

        public DispatcherStatsDto GetDispatcherStats()
        {
            // Stub: sample data
            return new DispatcherStatsDto
            {
                TotalActiveRequests = 12,
                TechsInTransit = 3,
                FollowUps = 2,
                Delays = 1,
                TopServiceType = "Plumbing"
            };
        }

        public List<RequestSummaryDto> GetFilteredRequests(DispatcherFilterModel filters)
        {
            // Stub: mock data
            var requests = new List<RequestSummaryDto>
            {
                new RequestSummaryDto { Id = 1, ServiceType = "Plumbing", Technician = "Alice Smith", Status = "Open", Priority = "High", Zip = "30301", DelayMinutes = 10 },
                new RequestSummaryDto { Id = 2, ServiceType = "Heating", Technician = "Bob Jones", Status = "Assigned", Priority = "Normal", Zip = "30302", DelayMinutes = 0 },
                new RequestSummaryDto { Id = 3, ServiceType = "Plumbing", Technician = "Carlos Lee", Status = "Delayed", Priority = "High", Zip = "30303", DelayMinutes = 25 },
                new RequestSummaryDto { Id = 4, ServiceType = "Heating", Technician = "Dana Patel", Status = "Follow-up", Priority = "Low", Zip = "30304", DelayMinutes = 0 },
                new RequestSummaryDto { Id = 5, ServiceType = "Plumbing", Technician = "Evan Kim", Status = "Open", Priority = "Normal", Zip = "30305", DelayMinutes = 5 }
            };
            if (!string.IsNullOrEmpty(filters.ServiceType))
                requests = requests.Where(r => r.ServiceType == filters.ServiceType).ToList();
            if (!string.IsNullOrEmpty(filters.Technician))
                requests = requests.Where(r => r.Technician == filters.Technician).ToList();
            if (!string.IsNullOrEmpty(filters.Status))
                requests = requests.Where(r => r.Status == filters.Status).ToList();
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                requests = filters.SortBy switch
                {
                    "Oldest" => requests.OrderBy(r => r.Id).ToList(),
                    "MostDelayed" => requests.OrderByDescending(r => r.DelayMinutes).ToList(),
                    "ZIP" => requests.OrderBy(r => r.Zip).ToList(),
                    _ => requests
                };
            }
            return requests;
        }

        public bool ReassignTechnician(int requestId, int newTechId)
        {
            // Stub: return true if IDs are positive
            return requestId > 0 && newTechId > 0;
        }
        public List<DispatcherNotification> GetNotifications()
        {
            return new List<DispatcherNotification>
            {
                new DispatcherNotification { Timestamp = DateTime.Now.AddMinutes(-2), Type = "Emergency", Message = "Emergency-flagged request #1023 requires immediate attention!" },
                new DispatcherNotification { Timestamp = DateTime.Now.AddMinutes(-10), Type = "Delay", Message = "Technician Bob Jones is delayed on job #1018." },
                new DispatcherNotification { Timestamp = DateTime.Now.AddMinutes(-15), Type = "Reassigned", Message = "Job #1015 reassigned to Dana Patel." },
                new DispatcherNotification { Timestamp = DateTime.Now.AddMinutes(-20), Type = "Info", Message = "Technician Carlos Lee marked inactive." }
            };
        }

        public void LogDispatcherAction(DispatcherAuditLog entry)
        {
            entry.Id = _auditLog.Count + 1;
            _auditLog.Add(entry);
        }
        public List<DispatcherAuditLog> GetAuditLog() => _auditLog.OrderByDescending(x => x.Timestamp).ToList();

        public List<MVP_Core.Models.Admin.DispatcherAuditLog> GetTimelineForRequest(int requestId)
        {
            return _auditLog
                .Where(x => x.RequestId == requestId)
                .OrderBy(x => x.Timestamp)
                .ToList();
        }

        public List<MVP_Core.Models.Admin.DispatcherAuditLog> GetReplayTimeline(int requestId)
        {
            return _auditLog
                .Where(x => x.RequestId == requestId)
                .OrderBy(x => x.Timestamp)
                .ToList();
        }

        public List<WatchdogAlert> RunWatchdogScan()
        {
            var now = DateTime.UtcNow;
            var alerts = new List<WatchdogAlert>();
            // Mocked requests
            var requests = new List<RequestSummaryDto>
            {
                new RequestSummaryDto { Id = 1, ServiceType = "Plumbing", Technician = "Alice Smith", Status = "Open", Priority = "High", Zip = "30301", DelayMinutes = 10 },
                new RequestSummaryDto { Id = 2, ServiceType = "Heating", Technician = "Bob Jones", Status = "Assigned", Priority = "Normal", Zip = "30302", DelayMinutes = 0 },
                new RequestSummaryDto { Id = 3, ServiceType = "Plumbing", Technician = "Carlos Lee", Status = "Delayed", Priority = "High", Zip = "30303", DelayMinutes = 25 },
                new RequestSummaryDto { Id = 4, ServiceType = "Heating", Technician = "Dana Patel", Status = "Follow-up", Priority = "Low", Zip = "30304", DelayMinutes = 0 },
                new RequestSummaryDto { Id = 5, ServiceType = "Plumbing", Technician = "Evan Kim", Status = "Open", Priority = "Normal", Zip = "30305", DelayMinutes = 5 }
            };
            // Mocked techs
            var techs = GetAllTechnicianStatuses();
            // ETA breach: DelayMinutes > 20
            foreach (var req in requests.Where(r => r.DelayMinutes > 20))
            {
                alerts.Add(new WatchdogAlert
                {
                    RequestId = req.Id,
                    AlertType = "ETAOverdue",
                    Message = $"ETA exceeded for job #{req.Id} ({req.Technician})",
                    DetectedAt = now
                });
            }
            // Inactivity: LastUpdate > 45 min ago
            foreach (var tech in techs.Where(t => (now - t.LastUpdate).TotalMinutes > 45))
            {
                alerts.Add(new WatchdogAlert
                {
                    RequestId = 0,
                    AlertType = "Inactivity",
                    Message = $"Technician {tech.Name} inactive for over 45 minutes.",
                    DetectedAt = now
                });
            }
            // Unassigned timeout: Open requests with no tech for 60+ min
            foreach (var req in requests.Where(r => r.Status == "Open" && string.IsNullOrEmpty(r.Technician) && r.DelayMinutes > 60))
            {
                alerts.Add(new WatchdogAlert
                {
                    RequestId = req.Id,
                    AlertType = "Unassigned",
                    Message = $"Request #{req.Id} unassigned for over 60 minutes.",
                    DetectedAt = now
                });
            }
            // Heartbeat overdue alerts
            foreach (var tech in _techHeartbeats.Where(t => (now - t.LastPing).TotalMinutes > 20))
            {
                alerts.Add(new WatchdogAlert
                {
                    RequestId = 0,
                    AlertType = "TechHeartbeatDrop",
                    Message = $"Technician {tech.Name} offline for over 20 minutes.",
                    DetectedAt = now
                });
            }
            return alerts;
        }
        public bool FlagEmergency(int requestId, string dispatcherName)
        {
            // Stub: find and mark request as emergency in mock list
            var req = GetFilteredRequests(new Models.Admin.DispatcherFilterModel()).FirstOrDefault(r => r.Id == requestId);
            if (req != null)
            {
                req.IsEmergency = true;
                req.DispatcherOverrideApplied = true;
                req.OverrideReason = "Dispatcher flagged emergency";
                LogDispatcherAction(new Models.Admin.DispatcherAuditLog
                {
                    ActionType = "Override-Emergency",
                    RequestId = requestId,
                    TechnicianId = null,
                    PerformedBy = dispatcherName,
                    Timestamp = DateTime.UtcNow,
                    Notes = req.OverrideReason
                });
                return true;
            }
            return false;
        }
        public DispatcherBroadcast? GetActiveBroadcast()
        {
            return _broadcasts.LastOrDefault(b => b.IsActive);
        }
        public void AddBroadcast(DispatcherBroadcast broadcast)
        {
            broadcast.Id = _broadcasts.Count + 1;
            _broadcasts.Add(broadcast);
        }

        public TechnicianProfileDto GetTechnicianProfile(int techId)
        {
            // Mock data for demonstration
            var tech = _techHeartbeats.FirstOrDefault(t => t.TechnicianId == techId);
            if (tech == null) return null;
            var rnd = new Random(techId);
            return new TechnicianProfileDto
            {
                TechnicianId = tech.TechnicianId,
                Name = tech.Name,
                CloseRate7Days = Math.Round(rnd.NextDouble() * 0.5 + 0.5, 2),
                CloseRate30Days = Math.Round(rnd.NextDouble() * 0.5 + 0.4, 2),
                CallbackCount7Days = rnd.Next(0, 4),
                TotalJobsLast30Days = rnd.Next(10, 40),
                TopZIPs = new[] { "30303", "30305", "30309" },
                Comments = new List<string> { "Great with customers.", "Needs to improve on-time rate." },
                LastActive = tech.LastPing
            };
        }

        public List<TechnicianStatusDto> GetSuggestedTechsBySkill(string requiredSkill, string zip)
        {
            // Stub: Use _techHeartbeats and mock profiles
            var profiles = _techHeartbeats.Select(t => new TechnicianProfileDto
            {
                TechnicianId = t.TechnicianId,
                Name = t.Name,
                SkillTags = new List<string> { "Plumbing", "Heating", "Air Conditioning", "Water Filtration" }, // stub
                TopZIPs = new[] { "30303", "30305", "30309" },
                LastActive = t.LastPing
            }).ToList();
            var filtered = profiles
                .Where(p => p.SkillTags.Contains(requiredSkill) && p.TopZIPs.Contains(zip))
                .OrderBy(p => (DateTime.UtcNow - p.LastActive).TotalMinutes)
                .ToList();
            if (!filtered.Any())
            {
                // Fallback: show all
                filtered = profiles;
            }
            // Map back to TechnicianStatusDto
            return _techHeartbeats.Where(t => filtered.Any(f => f.TechnicianId == t.TechnicianId)).ToList();
        }
        public void LogAssignment(AssignmentLogEntry entry)
        {
            entry.Id = _assignmentLogs.Count + 1;
            _assignmentLogs.Add(entry);
        }
        public List<AssignmentLogEntry> GetAssignmentLogs()
        {
            return _assignmentLogs.OrderByDescending(x => x.Timestamp).ToList();
        }

        public NextJobDto GetNextJobForTechnician(int techId)
        {
            var tech = _techHeartbeats.FirstOrDefault(t => t.TechnicianId == techId);
            if (tech == null)
                return null;
            // Stub: mock job
            return new NextJobDto
            {
                TechnicianId = tech.TechnicianId,
                TechnicianName = tech.Name,
                JobSummary = "A/C Install at Midtown Lofts",
                ETA = DateTime.UtcNow.AddMinutes(35),
                Address = "1234 Peachtree St NE, Atlanta, GA 30309",
                DispatcherNote = "Customer requests early arrival. Bring extra filters.",
                LastPing = tech.LastPing
            };
        }
        public void UpdateTechnicianPing(int techId)
        {
            var tech = _techHeartbeats.FirstOrDefault(t => t.TechnicianId == techId);
            if (tech != null)
                tech.LastPing = DateTime.UtcNow;
        }
    }

    public class DispatcherResult
    {
        public string Message { get; set; }
        public string RequestDetails { get; set; }
        public List<string> TechnicianList { get; set; }
        public string ETA { get; set; }
        public string GeoLink { get; set; }
        public string RequestSummary { get; set; }
        public string AssignedTechName { get; set; }
    }

    public class DispatcherStatsDto
    {
        public int TotalActiveRequests { get; set; }
        public int TechsInTransit { get; set; }
        public int FollowUps { get; set; }
        public int Delays { get; set; }
        public string TopServiceType { get; set; }
    }

    public class RequestSummaryDto
    {
        public int Id { get; set; }
        public string ServiceType { get; set; }
        public string Technician { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Zip { get; set; }
        public int DelayMinutes { get; set; }
        public bool IsEmergency { get; set; }
        public bool DispatcherOverrideApplied { get; set; }
        public string? OverrideReason { get; set; }
    }
    public class DispatcherNotification
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
    public class WatchdogAlert
    {
        public int RequestId { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public DateTime DetectedAt { get; set; }
    }
    public class TechnicianProfileDto
    {
        public int TechnicianId { get; set; }
        public string Name { get; set; }
        public double CloseRate7Days { get; set; }
        public double CloseRate30Days { get; set; }
        public int CallbackCount7Days { get; set; }
        public int TotalJobsLast30Days { get; set; }
        public string[] TopZIPs { get; set; }
        public List<string> Comments { get; set; }
        public DateTime LastActive { get; set; }
        public List<string> SkillTags { get; set; } // Added SkillTags property
    }
}
