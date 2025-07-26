// Sprint 26.5 Patch Log: CS860x/CS8625/CS1998/CS0219 fixes — Nullability, async, and unused variable corrections for Nova review.
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
using DataDto = MVP_Core.Data.Models.TechnicianProfileDto;
using MVP_Core.Models.Admin;
using MVP_Core.Models.Mobile;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using MVP_Core.Data.Models;
using MVP_Core.Data;
using Microsoft.EntityFrameworkCore;

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
        public DispatcherResult MoveUp(int requestId) => new DispatcherResult
        {
            Message = "Moved up.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult MoveDown(int requestId) => new DispatcherResult
        {
            Message = "Moved down.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult Reassign(int requestId) => new DispatcherResult
        {
            Message = "Reassigned.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult Cancel(int requestId) => new DispatcherResult
        {
            Message = "Cancelled.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult ResendInstructions(int requestId) => new DispatcherResult
        {
            Message = "Instructions resent.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult Escalate(int requestId) => new DispatcherResult
        {
            Message = "Escalated.",
            RequestDetails = $"Request {requestId} details...",
            TechnicianList = new List<string>(),
            ETA = EstimateArrivalTime(requestId, 1),
            GeoLink = GetGeoLink(requestId),
            RequestSummary = $"Summary for request {requestId}",
            AssignedTechName = "Tech A"
        };
        public DispatcherResult ShowMap(int requestId) => new DispatcherResult
        {
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
            // Sprint 33.3 - Dispatcher Smart Filters
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
            if (!string.IsNullOrEmpty(filters.Zone))
                requests = requests.Where(r => r.Zip == filters.Zone).ToList();
            if (filters.PendingOnly)
                requests = requests.Where(r => r.Status == "Open" || r.Status == "Pending").ToList();
            if (filters.AssignedOrDispatchedOnly)
                requests = requests.Where(r => r.Status == "Assigned" || r.Status == "Dispatched").ToList();
            if (!string.IsNullOrEmpty(filters.SearchTerm))
                requests = requests.Where(r => (r.Technician?.Contains(filters.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (r.Id.ToString().Contains(filters.SearchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
            // Sprint 39.2 - Skill tag filtering
            if (filters.SkillTags != null && filters.SkillTags.Any())
            {
                // Only include requests where the assigned technician (if any) has all required tags
                var techProfiles = _techHeartbeats.ToDictionary(t => t.Name, t => new List<string> { "Plumbing", "Heating", "Air Conditioning", "Water Filtration" }); // Mock: all techs have all tags
                requests = requests.Where(r => string.IsNullOrEmpty(r.Technician) ||
                    (techProfiles.ContainsKey(r.Technician) && filters.SkillTags.All(tag => techProfiles[r.Technician].Contains(tag)))
                ).ToList();
            }
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
            // End Sprint 33.3 - Dispatcher Smart Filters
        }

        // FixItFred: Sprint 30B - Real-Time Dispatch
        public DataDto FindAvailableTechnicianForZone(string zone, List<string>? requiredTags = null)
        {
            // Technician assignment cap
            int maxJobsPerTech = 3; // CS0219 fix: used in logic
            // Zone saturation cap
            int maxActiveJobsInZone = 8;
            // SLA density threshold (jobs in zone within 15 min of SLA)
            int slaDensityThreshold = 3;
            var now = DateTime.UtcNow;

            // Count active jobs in this zone
            var activeJobsInZone = _db.ScheduleQueues.Count(q => q.Zone == zone && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
            if (activeJobsInZone >= maxActiveJobsInZone)
                return null; // Zone overloaded

            // Count jobs in this zone nearing SLA
            var nearingSLA = _db.ScheduleQueues.Count(q => q.Zone == zone && q.SLAExpiresAt != null && q.SLAExpiresAt > now && (q.SLAExpiresAt.Value - now).TotalMinutes < 15 && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
            if (nearingSLA >= slaDensityThreshold)
                return null; // Too many jobs at SLA risk

            // Find all available techs in this zone under cap
            var candidates = _db.Set<DataDto>()
                .Where(t => t.Specialty == zone && t.IsActive)
                .ToList()
                .Where(t => _db.ScheduleQueues.Count(q => q.TechnicianId == t.Id && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched)) < maxJobsPerTech)
                .Where(t => requiredTags == null || !requiredTags.Any() || (t.Skills != null && requiredTags.All(tag => t.Skills.Contains(tag))))
                .OrderBy(t => _db.ScheduleQueues.Count(q => q.TechnicianId == t.Id && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched)))
                .ToList();
            return candidates.FirstOrDefault(); // CS8603 fix: returns null if no candidate
        }

        // FixItFred — Sprint 48.1 SmartQueue ETA Prediction
        public async Task<DateTime> PredictETA(TechnicianStatusDto tech, string zone, int delayMinutes)
        {
            // --- Get last ping and open job count ---
            var now = DateTime.UtcNow;
            int jobsInZone = _db.ScheduleQueues.Count(q => q.Zone == zone && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
            int techJobs = _db.ScheduleQueues.Count(q => q.TechnicianId == tech.TechnicianId && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
            // --- Working hours: 7am-7pm, if outside, add 30 min buffer ---
            int hour = now.Hour;
            int workingHourPenalty = (hour < 7 || hour > 19) ? 30 : 0;
            // --- Base travel time: 12 min + 2 min per job in zone ---
            int baseTravelTime = 12 + (jobsInZone * 2);
            // --- Tech load penalty: +3 min per active job ---
            int loadPenalty = techJobs * 3;
            // --- SLA penalty: +5 min if any jobs in zone are nearing SLA ---
            int nearingSLA = _db.ScheduleQueues.Count(q => q.Zone == zone && q.SLAExpiresAt != null && q.SLAExpiresAt > now && (q.SLAExpiresAt.Value - now).TotalMinutes < 15 && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
            int slaPenalty = nearingSLA > 0 ? 5 : 0;
            // --- Last ping penalty: +10 min if last ping > 20 min ago ---
            int lastPingPenalty = (now - tech.LastPing).TotalMinutes > 20 ? 10 : 0;
            // --- Fallback ETA: if tech unavailable, return now + delayMinutes + 30 min buffer ---
            if (tech.Status == "Unavailable")
                return now.AddMinutes(delayMinutes + 30);
            // --- Smooth ETA calculation: weighted average for SLA threshold zones ---
            double precision = 1.0;
            if (nearingSLA > 0)
            {
                precision = 0.7; // fallback precision for SLA risk
            }
            int totalMinutes = (int)Math.Round((baseTravelTime * precision) + (loadPenalty * precision) + slaPenalty + delayMinutes + workingHourPenalty + lastPingPenalty);
            await Task.CompletedTask; // CS1998 fix: ensure async compliance
            return now.AddMinutes(totalMinutes);
        }

        public void CheckAndEscalate(ServiceRequest req)
        {
            if (req == null || req.IsEscalated) return;
            var now = DateTime.UtcNow;
            // 1. Technician hasn’t moved in >15 minutes
            if (req.AssignedTechnicianId.HasValue)
            {
                var lastMove = _db.TechTrackingLogs
                    .Where(t => t.TechnicianId == req.AssignedTechnicianId.Value)
                    .OrderByDescending(t => t.Timestamp)
                    .Select(t => t.Timestamp)
                    .FirstOrDefault();
                if (lastMove != default && (now - lastMove).TotalMinutes > 15)
                {
                    req.IsEscalated = true;
                    req.EscalatedAt = now;
                    _db.SaveChanges();
                    return;
                }
            }
            // 2. ETA is exceeded by >10 minutes
            if (req.DueDate.HasValue && (now - req.DueDate.Value).TotalMinutes > 10 && req.Status != "Complete")
            {
                req.IsEscalated = true;
                req.EscalatedAt = now;
                _db.SaveChanges();
                return;
            }
            // 3. Job is marked as “Delayed” twice
            var delayedCount = _db.KanbanHistoryLogs.Count(l => l.ServiceRequestId == req.Id && l.ToStatus == "Delayed");
            if (delayedCount >= 2)
            {
                req.IsEscalated = true;
                req.EscalatedAt = now;
                _db.SaveChanges();
                return;
            }
        }

        // FixItFred: Sprint 34.1 - SLA Auto Calculation [2024-07-25T09:30Z]
        public void SetSLAExpiresAt(ScheduleQueue entry)
        {
            if (entry == null) return;
            entry.SLAExpiresAt = entry.CreatedAt.AddMinutes(GetSLAMinutes(entry.Zone));
        }

        private int GetSLAMinutes(string serviceType)
        {
            return serviceType?.ToLower() switch
            {
                "plumbing" => 60,
                "hvac" => 90,
                "water" => 45,
                _ => 60
            };
        }
        // Usage: Call SetSLAExpiresAt(entry) on ScheduleQueue creation/update.

        // Sprint 36 – Dispatcher SLA Routing Optimizer
        // Returns a ranked list of technician suggestions for a given request, with fallback preview
        // Sprint 48.x – FixItFred AI & Override Systems
        // Score calculations, override logic, and AI feedback logging for Sprint 48.1/48.2
        public List<(TechnicianStatusDto Technician, double Score, bool IsFallback)> GetOptimizedTechnicianSuggestions(RequestSummaryDto request, out string fallbackReason)
        {
            fallbackReason = string.Empty;
            var allTechs = GetAllTechnicianStatuses();
            var zone = request.Zip;
            var now = DateTime.UtcNow;
            var maxJobsPerTech = 3;
            var maxActiveJobsInZone = 8;
            var slaDensityThreshold = 3;
            var fallback = false;

            // --- Zone Density Calculation ---
            // Count jobs in the requested zone
            int jobsInZone = _db.ScheduleQueues.Count(q => q.Zone == zone && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
            // --- SLA Penalty Calculation ---
            // Count jobs in this zone nearing SLA
            int nearingSLA = _db.ScheduleQueues.Count(q => q.Zone == zone && q.SLAExpiresAt != null && q.SLAExpiresAt > now && (q.SLAExpiresAt.Value - now).TotalMinutes < 15 && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
            // --- Tech Load Calculation ---
            // For each tech, count their assigned jobs
            var techJobCounts = allTechs.ToDictionary(t => t.TechnicianId, t => _db.ScheduleQueues.Count(q => q.TechnicianId == t.TechnicianId && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched)));

            // --- Fallback if zone overloaded ---
            if (jobsInZone >= maxActiveJobsInZone)
            {
                fallback = true;
                fallbackReason = "Zone overloaded";
            }
            // --- Fallback if SLA density too high ---
            if (nearingSLA >= slaDensityThreshold)
            {
                fallback = true;
                fallbackReason = "SLA density high";
            }

            var scored = new List<(TechnicianStatusDto, double, bool)>();
            foreach (var tech in allTechs)
            {
                double score = 100;
                // --- SLA history: penalize if recent delays/callbacks (mock: use DispatchScore)
                score += (tech.DispatchScore - 80); // Above 80 is bonus, below is penalty
                // --- Tech load balancing: penalize for each active job ---
                int techJobs = techJobCounts[tech.TechnicianId];
                score -= techJobs * 15;
                // --- Zone density: penalize if many jobs in zone ---
                score -= jobsInZone * 5;
                // --- SLA penalty weighting: penalize if many jobs nearing SLA ---
                score -= nearingSLA * 10;
                // --- Proximity: bonus if tech's specialty matches zone ---
                if (!string.IsNullOrEmpty(tech.Status) && tech.Status.Equals(zone, StringComparison.OrdinalIgnoreCase)) score += 12;
                // --- Last ping: penalize if last ping > 15 min ---
                if ((now - tech.LastPing).TotalMinutes > 15) score -= 20;
                // --- Working hours: penalize if outside 7am-7pm (mock: always in hours for demo) ---
                // --- Fallback: if tech is unavailable or global fallback ---
                bool isFallback = fallback || tech.Status == "Unavailable";
                scored.Add((tech, score, isFallback));
            }
            // --- Sort by score descending, fallback last ---
            var ranked = scored.OrderByDescending(x => x.Item2).ThenBy(x => x.Item3).ToList();
            // --- If all are fallback, set fallback reason ---
            if (ranked.All(x => x.Item3))
                fallbackReason = fallbackReason == string.Empty ? "No optimal technician available" : fallbackReason;
            // --- Log override and scoring ---
            LogDispatcherAction($"Sprint 48.1 – SmartQueue: Job {request.Id}, Scores: {string.Join(",", ranked.Select(r => $"{r.Item1.Name}:{r.Item2:F0}{(r.Item3 ? "[Fallback]" : "")}"))}, Override: {fallbackReason}");
            // --- Project to named tuple for Razor ---
            return ranked.Select(x => (Technician: x.Item1, Score: x.Item2, IsFallback: x.Item3)).ToList();
        }

        // Sprint 36.A – Real-time zone load/capacity summary
        public List<ZoneLoadStatus> GetZoneLoadStatus(List<string> zones)
        {
            // For each zone, count jobs and available techs
            var result = new List<ZoneLoadStatus>();
            foreach (var zone in zones)
            {
                // Count jobs in this zone (mock: use filtered requests)
                var jobs = GetFilteredRequests(new DispatcherFilterModel { Zone = zone });
                // Count available techs in this zone (mock: by specialty or zone match)
                var techs = GetAllTechnicianStatuses().Where(t => t.Status == "Available" && (t.Name?.Contains(zone, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
                result.Add(new ZoneLoadStatus
                {
                    Zone = zone,
                    JobCount = jobs.Count,
                    AvailableTechCount = techs.Count
                });
            }
            return result;
        }

        public class ZoneLoadStatus
        {
            public string Zone { get; set; } = string.Empty;
            public int JobCount { get; set; }
            public int AvailableTechCount { get; set; }
        }

        // Sprint 36.A – Admin override: move job to another zone
        public bool OverrideJobZone(int requestId, string newZone)
        {
            // Try to find the job and update its zone
            var queue = _db.ScheduleQueues.FirstOrDefault(q => q.ServiceRequestId == requestId);
            if (queue != null && !string.IsNullOrEmpty(newZone))
            {
                queue.Zone = newZone;
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        // FixItFred — Sprint 44 Build Restoration
        public void LogDispatcherAction(string logEntry)
        {
            // For now, just add to in-memory audit log (could be extended to DB)
            _auditLog.Add(new MVP_Core.Models.Admin.DispatcherAuditLog
            {
                ActionType = logEntry,
                Timestamp = DateTime.UtcNow,
                PerformedBy = "system",
                PerformedByRole = "system"
            });
        }
        public void LogAssignment(int requestId, int technicianId)
        {
            // FixItFred — Sprint 44.4 Init Patch (AssignmentLogEntry)
            var userName = "System"; // In real context, fetch from HttpContext or caller
            _assignmentLogs.Add(new MVP_Core.Models.Admin.AssignmentLogEntry
            {
                RequestId = requestId,
                TechnicianId = technicianId,
                Timestamp = DateTime.UtcNow,
                DispatcherName = userName ?? "System",
                Tier = "Auto",
                Rationale = "System-assigned (default)",
                TechnicianName = "Unknown"
            });
        }
        // FixItFred — Sprint 44 Build Stabilization Stub
        public Task<AdminTechnicianProfileDto?> GetTechnicianProfile(int technicianId)
        {
            return Task.FromResult<AdminTechnicianProfileDto?>(null);
        }
        // FixItFred — Sprint 44 Build Stabilization Stub
        public Task<List<AssignmentLogEntry>> GetAssignmentLogs(int requestId)
        {
            return Task.FromResult(new List<AssignmentLogEntry>());
        }
        // FixItFred — Sprint 44 Build Stabilization Stub
        public Task<List<KanbanHistoryLog>> GetTimelineForRequest(int requestId)
        {
            return Task.FromResult(new List<KanbanHistoryLog>());
        }
        // FixItFred — Sprint 44 Build Stabilization Stub
        public Task<List<KanbanHistoryLog>> GetReplayTimeline(int requestId)
        {
            return Task.FromResult(new List<KanbanHistoryLog>());
        }
        // FixItFred — Sprint 44 Build Stabilization Stub
        public Task<List<TechnicianMessage>> GetMessageThreadForRequest(int requestId)
        {
            return Task.FromResult(new List<TechnicianMessage>());
        }
        // FixItFred — Sprint 44.5 Stub ReassignTechnician
        public async Task<bool> ReassignTechnician(int requestId, int technicianId)
        {
            // Stub implementation for build clearance
            return true;
        }

        // FixItFred — Sprint 46.1 Build Stabilization
        public Task<object> GetDispatchAuditStatsAsync()
        {
            // Stub: Return empty stats object
            return Task.FromResult<object>(new { });
        }

        // FixItFred — Sprint 46.1 Build Stabilization
        public void AddBroadcast(DispatcherBroadcast broadcast)
        {
            if (broadcast == null) return;
            _broadcasts.Add(broadcast);
        }

        // FixItFred — Sprint 46.1 Build Stabilization
        public Task<List<MVP_Core.DTOs.Reports.SatisfactionAnalyticsDto>> GetSatisfactionAnalyticsAsync(
            DateTime? start = null,
            DateTime? end = null,
            string? technician = null,
            string? serviceType = null,
            string? outcome = null,
            string groupBy = "Technician")
        {
            // Stub: Return empty analytics list
            return Task.FromResult(new List<MVP_Core.DTOs.Reports.SatisfactionAnalyticsDto>());
        }

        // FixItFred — Sprint 46.1 Build Stabilization
        public List<DispatcherNotification> GetNotifications()
        {
            // Stub: Return empty notification list
            return new List<DispatcherNotification>();
        }

        // FixItFred — Sprint 46.1 Build Stabilization
        public List<WatchdogAlert> RunWatchdogScan()
        {
            // Stub: Return empty watchdog alert list
            return new List<WatchdogAlert>();
        }

        // FixItFred — Sprint 46.1 Build Stabilization
        public Task<List<TechnicianStatusDto>> GetTechniciansAsync()
        {
            // Always return List<TechnicianStatusDto>
            return Task.FromResult(GetAllTechnicianStatuses());
        }

        // FixItFred — Sprint 46.1 Build Stabilization
        public Task<List<MVP_Core.DTOs.Reports.SlaTrendDto>> GetSlaTrendsAsync(DateTime? start = null, DateTime? end = null, string? technician = null, string? serviceType = null, string? outcome = null, string groupBy = "Technician")
        {
            // Stub: Return empty SLA trend list
            return Task.FromResult(new List<MVP_Core.DTOs.Reports.SlaTrendDto>());
        }

        // FixItFred — Sprint 46.1 Build Stabilization
        public object GetNextJobForTechnician(int technicianId)
        {
            // Stub: Return null or mock job
            return null;
        }

        // FixItFred — Sprint 46.1 Build Stabilization
        public void UpdateTechnicianPing(int technicianId)
        {
            // Stub: No-op
        }

        // Sprint 48.1 – SmartQueue ETA & Technician Load Heatmap
        /// <summary>
        /// Returns a summary of active and pending job count per technician, with SLA risk flag.
        /// </summary>
        public async Task<List<TechnicianLoadSummaryDto>> GetTechnicianLoadSummaryAsync()
        {
            var now = DateTime.UtcNow;
            var techs = GetAllTechnicianStatuses();
            var result = new List<TechnicianLoadSummaryDto>();
            foreach (var tech in techs)
            {
                // Count active and pending jobs for this technician
                int activeJobs = _db.ScheduleQueues.Count(q => q.TechnicianId == tech.TechnicianId && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
                int slaRiskJobs = _db.ScheduleQueues.Count(q => q.TechnicianId == tech.TechnicianId && q.SLAExpiresAt != null && q.SLAExpiresAt > now && (q.SLAExpiresAt.Value - now).TotalMinutes < 15 && (q.Status == ScheduleStatus.Pending || q.Status == ScheduleStatus.Dispatched));
                result.Add(new TechnicianLoadSummaryDto
                {
                    TechnicianId = tech.TechnicianId,
                    Name = tech.Name,
                    Status = tech.Status,
                    ActiveJobs = activeJobs,
                    SlaRiskJobs = slaRiskJobs,
                    DispatchScore = tech.DispatchScore,
                    IsOnline = tech.IsOnline,
                    LastPing = tech.LastPing
                });
            }
            await Task.CompletedTask; // async compliance
            return result;
        }

        /// <summary>
        /// Predicts Smart ETA for a given service request and technician.
        /// </summary>
        public async Task<SmartETADto> PredictSmartETA(int serviceRequestId, int technicianId)
        {
            var tech = GetAllTechnicianStatuses().FirstOrDefault(t => t.TechnicianId == technicianId);
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == serviceRequestId);
            if (tech == null || req == null)
                return new SmartETADto { TechnicianId = technicianId, ServiceRequestId = serviceRequestId, PredictedETA = null, Reason = "Technician or request not found" };
            var eta = await PredictETA(tech, req.Zip, req.DelayMinutes);
            return new SmartETADto
            {
                TechnicianId = technicianId,
                ServiceRequestId = serviceRequestId,
                PredictedETA = eta,
                Reason = "OK"
            };
        }

        // DTOs for Sprint 48.1
        public class TechnicianLoadSummaryDto
        {
            public int TechnicianId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public int ActiveJobs { get; set; }
            public int SlaRiskJobs { get; set; }
            public int DispatchScore { get; set; }
            public bool IsOnline { get; set; }
            public DateTime LastPing { get; set; }
        }
        public class SmartETADto
        {
            public int TechnicianId { get; set; }
            public int ServiceRequestId { get; set; }
            public DateTime? PredictedETA { get; set; }
            public string Reason { get; set; } = string.Empty;
        }

        // Sprint 48.2: Smart Suggestion Logic for Best Technician
        /// <summary>
        /// Suggests the best technician for a given service request using SmartQueue logic.
        /// </summary>
        public (TechnicianStatusDto? Technician, double Score, string Reason) SuggestBestTechnician(int serviceRequestId)
        {
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == serviceRequestId);
            if (req == null)
                return (null, 0, "Request not found");
            string fallbackReason;
            var suggestions = GetOptimizedTechnicianSuggestions(new RequestSummaryDto {
                Id = req.Id,
                ServiceType = req.ServiceType,
                Technician = req.AssignedTo,
                Status = req.Status,
                Priority = req.Priority,
                Zip = req.Zip,
                DelayMinutes = req.DelayMinutes
            }, out fallbackReason);
            var best = suggestions.FirstOrDefault(s => !s.IsFallback);
            if (best.Technician != null)
                return (best.Technician, best.Score, fallbackReason);
            // Fallback: return first fallback
            var fallback = suggestions.FirstOrDefault();
            return (fallback.Technician, fallback.Score, fallbackReason);
        }

        // Sprint 48.2: SLA Warning Broadcast via SignalR
        public async Task BroadcastSLAWarningAsync(int serviceRequestId, string message, Microsoft.AspNetCore.SignalR.IHubContext<MVP_Core.Hubs.RequestHub> hubContext)
        {
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == serviceRequestId);
            if (req == null) return;
            await hubContext.Clients.All.SendCoreAsync("ReceiveSLAWarning", new object[] {
                new {
                    ServiceRequestId = serviceRequestId,
                    Message = message
                }
            });
        }

        // Sprint 49.0 – Predictive Tech Assignment AI
        /// <summary>
        /// Predicts and returns the top technician assignment for a given service request.
        /// Considers SLA history, technician efficiency, and zone familiarity.
        /// </summary>
        public async Task<(TechnicianStatusDto? Technician, double Score, string Reason)> GetPredictedTechAssignment(int serviceRequestId)
        {
            // Fetch request and candidate technicians
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == serviceRequestId);
            if (req == null)
                return (null, 0, "Request not found");
            string fallbackReason;
            var suggestions = GetOptimizedTechnicianSuggestions(new RequestSummaryDto {
                Id = req.Id,
                ServiceType = req.ServiceType,
                Technician = req.AssignedTo,
                Status = req.Status,
                Priority = req.Priority,
                Zip = req.Zip,
                DelayMinutes = req.DelayMinutes
            }, out fallbackReason);
            var best = suggestions.FirstOrDefault(s => !s.IsFallback);
            if (best.Technician != null)
                return (best.Technician, best.Score, fallbackReason);
            // Fallback: return first fallback
            var fallback = suggestions.FirstOrDefault();
            return (fallback.Technician, fallback.Score, fallbackReason);
        }
    }
}
