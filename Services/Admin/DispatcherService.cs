using MVP_Core.Data.Models;
using MVP_Core.Models.Admin;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MVP_Core.Services.Admin
{
    public class DispatcherService
    {
        private static List<DispatcherAuditLog> _auditLog = new();

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
    public class DispatcherAuditLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
    public class WatchdogAlert
    {
        public int RequestId { get; set; }
        public string AlertType { get; set; }
        public string Message { get; set; }
        public DateTime DetectedAt { get; set; }
    }
}
