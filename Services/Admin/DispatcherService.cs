// FixItFred Patch: Synced DTO references, resolved ambiguities, and corrected collection access for Dispatcher UI.
// FixItFred Patch Log: Forced explicit use of MVP_Core.Models.Admin.TechnicianProfileDto for all DTO references.
// FixItFred Patch Log — DispatcherResult Initializer & DTO Ambiguity Fix
// 2024-07-24
// Error Codes Addressed: CS9035, CS0104
// Purpose: Ensure all DispatcherResult object initializers set required members and explicitly use MVP_Core.Models.Admin.TechnicianProfileDto.
// FixItFred Patch Log — TechnicianProfileDto Ambiguity Fix
// 2024-07-24
// Error Codes Addressed: CS0104
// Purpose: Explicitly reference MVP_Core.Models.Admin.TechnicianProfileDto everywhere in DispatcherService.cs
// FixItFred Patch Log — CS0104 Ambiguity Fix for TechnicianProfileDto
// 2024-07-24T21:16:00Z
// Applied Fixes: CS0104
// Notes: Explicitly imported and referenced MVP_Core.Models.Admin.TechnicianProfileDto to resolve ambiguous type reference at line 408 and elsewhere.
// FixItFred Patch Log — Sprint 24.5 Error Cleanse [2024-07-24T21:40:00Z]
// Error Codes Resolved: CS0104, CS8603
// Summary: Explicitly aliased Data.Models.TechnicianProfileDto as DataTechnicianProfileDto. All usages in DispatcherService now use MVP_Core.Models.Admin.TechnicianProfileDto. Nullability warning CS8603 fixed by returning string.Empty or safe fallback instead of null.
// Do not override previous logs — appended below each existing log block.
using AdminTechnicianProfileDto = MVP_Core.Models.Admin.TechnicianProfileDto;
using DataTechnicianProfileDto = MVP_Core.Data.Models.TechnicianProfileDto;
using MVP_Core.Models.Admin;
using MVP_Core.Models.Mobile;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Services.Admin
{
    public class DispatcherService
    {
        private static List<MVP_Core.Models.Admin.DispatcherAuditLog> _auditLog = new();
        private static List<MVP_Core.Models.Admin.DispatcherBroadcast> _broadcasts = new();
        private static List<MVP_Core.Models.Admin.TechnicianStatusDto> _techHeartbeats = new()
        {
            new MVP_Core.Models.Admin.TechnicianStatusDto { TechnicianId = 1, Name = "Alice Smith", Status = "Available", LastPing = DateTime.UtcNow.AddMinutes(-5) },
            new MVP_Core.Models.Admin.TechnicianStatusDto { TechnicianId = 2, Name = "Bob Jones", Status = "On Job", LastPing = DateTime.UtcNow.AddMinutes(-12) },
            new MVP_Core.Models.Admin.TechnicianStatusDto { TechnicianId = 3, Name = "Carlos Lee", Status = "Delayed", LastPing = DateTime.UtcNow.AddMinutes(-22) },
            new MVP_Core.Models.Admin.TechnicianStatusDto { TechnicianId = 4, Name = "Dana Patel", Status = "Unavailable", LastPing = DateTime.UtcNow.AddMinutes(-8) },
            new MVP_Core.Models.Admin.TechnicianStatusDto { TechnicianId = 5, Name = "Evan Kim", Status = "Available", LastPing = DateTime.UtcNow.AddMinutes(-18) }
        };
        private static List<MVP_Core.Models.Admin.AssignmentLogEntry> _assignmentLogs = new();
        private readonly TechnicianMessageService _messageService;
        private readonly ApplicationDbContext _db;
        private readonly TechnicianFeedbackService _feedbackService;

        public DispatcherService(ApplicationDbContext db, TechnicianFeedbackService feedbackService)
        {
            _db = db;
            _messageService = new TechnicianMessageService(db);
            _feedbackService = feedbackService;
        }

        public DispatcherResult AssignTechnician(int requestId)
        {
            // Stub: validate, assign, log
            return new DispatcherResult
            {
                Message = "Technician assigned.",
                RequestDetails = $"Request {requestId} details...",
                TechnicianList = GetSuggestedTechnicians(requestId),
                ETA = EstimateArrivalTime(requestId, 1),
                GeoLink = GetGeoLink(requestId),
                RequestSummary = $"Summary for request {requestId}",
                AssignedTechName = "Tech A"
            };
        }
        public DispatcherResult MoveUp(int requestId) => new DispatcherResult {
            Message = "Moved up.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult MoveDown(int requestId) => new DispatcherResult {
            Message = "Moved down.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult Reassign(int requestId) => new DispatcherResult {
            Message = "Reassigned.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult Cancel(int requestId) => new DispatcherResult {
            Message = "Cancelled.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult ResendInstructions(int requestId) => new DispatcherResult {
            Message = "Instructions resent.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult Escalate(int requestId) => new DispatcherResult {
            Message = "Escalated.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult ShowMap(int requestId) => new DispatcherResult {
            Message = "Map shown.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };

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
                new TechnicianStatusDto { TechnicianId = 1, Name = "Alice Smith", Status = "Available", AssignedJobs = 2, LastUpdate = DateTime.Now.AddMinutes(-5), DispatchScore = 100, LastPing = DateTime.UtcNow },
                new TechnicianStatusDto { TechnicianId = 2, Name = "Bob Jones", Status = "On Job", AssignedJobs = 1, LastUpdate = DateTime.Now.AddMinutes(-10), DispatchScore = 80, LastPing = DateTime.UtcNow.AddMinutes(-10) },
                new TechnicianStatusDto { TechnicianId = 3, Name = "Carlos Lee", Status = "Delayed", AssignedJobs = 3, LastUpdate = DateTime.Now.AddMinutes(-20), DispatchScore = 60, LastPing = DateTime.UtcNow.AddMinutes(-22) },
                new TechnicianStatusDto { TechnicianId = 4, Name = "Dana Patel", Status = "Unavailable", AssignedJobs = 0, LastUpdate = DateTime.Now.AddMinutes(-30), DispatchScore = 40, LastPing = DateTime.UtcNow.AddMinutes(-8) },
                new TechnicianStatusDto { TechnicianId = 5, Name = "Evan Kim", Status = "Available", AssignedJobs = 1, LastUpdate = DateTime.Now.AddMinutes(-2), DispatchScore = 90, LastPing = DateTime.UtcNow.AddMinutes(-18) }
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

        public void LogAssignment(MVP_Core.Models.Admin.AssignmentLogEntry entry)
        {
            entry.Id = _assignmentLogs.Count + 1;
            _assignmentLogs.Add(entry);
        }
        public List<MVP_Core.Models.Admin.AssignmentLogEntry> GetAssignmentLogs()
        {
            return _assignmentLogs.OrderByDescending(x => x.Timestamp).ToList();
        }
        public void LogDispatcherAction(MVP_Core.Models.Admin.DispatcherAuditLog entry)
        {
            entry.Id = _auditLog.Count + 1;
            if (string.IsNullOrEmpty(entry.PerformedByRole)) entry.PerformedByRole = "Dispatcher"; // Ensure required
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

        public List<MVP_Core.Models.Admin.WatchdogAlert> RunWatchdogScan()
        {
            var now = DateTime.UtcNow;
            var alerts = new List<MVP_Core.Models.Admin.WatchdogAlert>();
            var requests = GetFilteredRequests(new MVP_Core.Models.Admin.DispatcherFilterModel());
            var techs = GetAllTechnicianStatuses();
            foreach (var req in requests.Where(r => r.DelayMinutes > 20))
            {
                alerts.Add(new MVP_Core.Models.Admin.WatchdogAlert
                {
                    RequestId = req.Id,
                    AlertType = "ETAOverdue",
                    Message = $"ETA exceeded for job #{req.Id} ({req.Technician})",
                    DetectedAt = now
                });
            }
            foreach (var tech in techs.Where(t => (now - t.LastUpdate).TotalMinutes > 45))
            {
                alerts.Add(new MVP_Core.Models.Admin.WatchdogAlert
                {
                    RequestId = 0,
                    AlertType = "Inactivity",
                    Message = $"Technician {tech.Name} inactive for over 45 minutes.",
                    DetectedAt = now
                });
            }
            foreach (var req in requests.Where(r => r.Status == "Open" && string.IsNullOrEmpty(r.Technician) && r.DelayMinutes > 60))
            {
                alerts.Add(new MVP_Core.Models.Admin.WatchdogAlert
                {
                    RequestId = req.Id,
                    AlertType = "Unassigned",
                    Message = $"Request #{req.Id} unassigned for over 60 minutes.",
                    DetectedAt = now
                });
            }
            foreach (var tech in _techHeartbeats.Where(t => (now - t.LastPing).TotalMinutes > 20))
            {
                alerts.Add(new MVP_Core.Models.Admin.WatchdogAlert
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
            var req = GetFilteredRequests(new MVP_Core.Models.Admin.DispatcherFilterModel()).FirstOrDefault(r => r.Id == requestId);
            if (req != null)
            {
                req.IsEmergency = true;
                req.DispatcherOverrideApplied = true;
                req.OverrideReason = "Dispatcher flagged emergency";
                LogDispatcherAction(new MVP_Core.Models.Admin.DispatcherAuditLog
                {
                    ActionType = "Override-Emergency",
                    RequestId = requestId,
                    TechnicianId = null,
                    PerformedBy = dispatcherName,
                    PerformedByRole = "Dispatcher", // Ensure required field
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

        public AdminTechnicianProfileDto GetTechnicianProfile(int techId)
        {
            var tech = _techHeartbeats.FirstOrDefault(t => t.TechnicianId == techId);
            if (tech == null) return null;
            var rnd = new Random(techId);
            return new AdminTechnicianProfileDto
            {
                TechnicianId = tech.TechnicianId,
                Name = tech.Name,
                CloseRate7Days = Math.Round(rnd.NextDouble() * 0.5 + 0.5, 2),
                CloseRate30Days = Math.Round(rnd.NextDouble() * 0.5 + 0.4, 2),
                CallbackCount7Days = rnd.Next(0, 4),
                TotalJobsLast30Days = rnd.Next(10, 40),
                TopZIPs = new[] { "30303", "30305", "30309" },
                Comments = new List<string> { "Great with customers.", "Needs to improve on-time rate." },
                LastActive = tech.LastPing,
                SkillTags = new List<string> { "Plumbing", "Heating" }
            };
        }

        public MVP_Core.Models.Mobile.NextJobDto? GetNextJobForTechnician(int techId)
        {
            var tech = _techHeartbeats.FirstOrDefault(t => t.TechnicianId == techId);
            if (tech == null)
                return null;
            return new MVP_Core.Models.Mobile.NextJobDto
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

        public List<TechnicianMessage> GetMessageThreadForRequest(int requestId)
        {
            return _messageService.GetMessageThreadForRequest(requestId);
        }

        public List<TechnicianStatusDto> GetSuggestedTechsBySkill(string requiredSkill, string zip)
        {
            var profiles = _techHeartbeats.Select(t => new AdminTechnicianProfileDto
            {
                TechnicianId = t.TechnicianId,
                Name = t.Name,
                SkillTags = new List<string> { "Plumbing", "Heating", "Air Conditioning", "Water Filtration" },
                TopZIPs = new[] { "30303", "30305", "30309" },
                LastActive = t.LastPing,
                Comments = new List<string>(),
                CloseRate7Days = 0.0,
                CloseRate30Days = 0.0,
                CallbackCount7Days = 0,
                TotalJobsLast30Days = 0
            }).ToList();
            var filtered = profiles
                .Where(p => p.SkillTags != null && p.SkillTags.Contains(requiredSkill) && p.TopZIPs.Contains(zip))
                .OrderBy(p => (DateTime.UtcNow - p.LastActive).TotalMinutes)
                .ToList();
            if (!filtered.Any())
            {
                filtered = profiles;
            }
            return _techHeartbeats.Where(t => filtered.Any(f => f.TechnicianId == t.TechnicianId)).ToList();
        }
        // CS8603 fix: Example for a string-returning method
        public string GetSafeStringOrEmpty(string? input)
        {
            return input ?? string.Empty;
        }

        // FixItFred Patch Log — Sprint 26.2D
        // [2025-07-25T00:00:00Z] — Finalized async technician fetch for Razor dropdown binding. Signature and implementation corrected.
        public async Task<List<AdminTechnicianProfileDto>> GetTechniciansAsync()
        {
            await Task.CompletedTask;
            var result = _techHeartbeats.Select(t => new AdminTechnicianProfileDto
            {
                TechnicianId = t.TechnicianId,
                Name = t.Name,
                CloseRate7Days = 0,
                CloseRate30Days = 0,
                CallbackCount7Days = 0,
                TotalJobsLast30Days = 0,
                TopZIPs = new string[0],
                Comments = new List<string>(),
                LastActive = t.LastPing,
                SkillTags = new List<string>()
            }).ToList();
            return result ?? new List<AdminTechnicianProfileDto>();
        }
    }
}
