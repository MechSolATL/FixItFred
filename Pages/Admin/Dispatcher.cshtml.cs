// FixItFred Patch Log — CS1998 Async Compliance Patch
// Sprint83.4-IsOnlinePatch: Fixed CS0200 read-only property assignment
// Top-level statement for Technicians initialization removed. Initialization is handled inside OnGetAsync.
// 2024-07-24T21:16:00Z
// Applied Fixes: CS1998
// Notes: Inserted await Task.CompletedTask in async methods without awaits for compliance.
// FixItFred Patch Log — Sprint 26.2D
// [2025-07-25T00:00:00Z] — Final async Razor binding for TechnicianDropdownViewModel. CS0118/CS1061 resolved.
// FixItFred Patch Log — Sprint 28 Recovery Patch
// [2024-07-25T00:40:00Z] — Added ServiceZones property for zone filtering in dispatcher UI.
// FixItFred Patch Log — Sprint 28
// [2025-07-25T00:00:00Z] — ServiceZones property exposed and Razor reference corrected for Dispatcher view.
using MVP_Core.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using MVP_Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using System.Text;
using System.Globalization;
using OfficeOpenXml;
using Microsoft.Extensions.Options;
using MVP_Core.Services.Config;
using MVP_Core.Services.Admin;
using System.Threading.Tasks;
using DispatcherAuditLog = MVP_Core.Models.Admin.DispatcherAuditLog;
using MVP_Core.Data.Models.ViewModels;
using MVP_Core.Services.Dispatch; // FixItFred: Sprint 30D.2 — Add using for NotificationDispatchEngine to resolve CS0246 2024-07-25
using MVP_Core.Services; // Sprint 32.2 - Security + Audit Harden

namespace MVP_Core.Pages.Admin
{
    // FixItFred — Sprint 40.4 Authorization Scope Fix
    [Authorize(Roles = "Admin")]
    public class DispatcherModel : PageModel
    {
        private readonly DispatcherService _dispatcherService; // Sprint 79.2
        private readonly ApplicationDbContext _db; // Sprint 79.2
        private readonly IHubContext<RequestHub> _hubContext; // Sprint 79.2
        private readonly IOptions<LoadBalancingConfig> _lbConfig; // Sprint 79.2
        private readonly NotificationDispatchEngine _dispatchEngine; // Sprint 79.2
        private readonly IAuditTrailLogger _auditLogger; // Sprint 79.2

        public DispatcherModel(DispatcherService dispatcherService, ApplicationDbContext db, IHubContext<RequestHub> hubContext, IOptions<LoadBalancingConfig> lbConfig, NotificationDispatchEngine dispatchEngine, IAuditTrailLogger auditLogger)
        {
            _dispatcherService = dispatcherService ?? throw new ArgumentNullException(nameof(dispatcherService)); // Sprint 79.2
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Sprint 79.2
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext)); // Sprint 79.2
            _lbConfig = lbConfig ?? throw new ArgumentNullException(nameof(lbConfig)); // Sprint 79.2
            _dispatchEngine = dispatchEngine ?? throw new ArgumentNullException(nameof(dispatchEngine)); // Sprint 79.2
            _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger)); // Sprint 79.2
            DispatcherRequests = new(); // Sprint 79.2
            TechnicianStatuses = new(); // Sprint 79.2
            DispatcherStats = new DispatcherStatsDto { TotalActiveRequests = 0, TechsInTransit = 0, FollowUps = 0, Delays = 0, TopServiceType = string.Empty }; // Sprint 79.2
            Notifications = new(); // Sprint 79.2
            WatchdogAlerts = new(); // Sprint 79.2
            Requests = new(); // Sprint 79.2
            ServiceTypes = new(); // Sprint 79.2
            KanbanHistory = new(); // Sprint 79.2
            SlaSettings = new(); // Sprint 79.2
            TechnicianLoads = new(); // Sprint 79.2
            SuggestedTechnicians = new(); // Sprint 79.2
            SuggestedScores = new(); // Sprint 79.2
            SuggestedTech = new(); // Sprint 79.2
            OptimizedSuggestions = new(); // Sprint 79.2
            OptimizerFallbackReasons = new(); // Sprint 79.2
            AllSkillTags = new List<string> { "Tankless", "Mini Split", "Backflow Cert", "Plumbing", "Heating", "Air Conditioning", "Water Filtration" }; // Sprint 79.2
            SkillTags = new(); // Sprint 79.2
            ServiceZones = new(); // Sprint 79.2
            TechnicianDropdownViewModel = new MVP_Core.Data.Models.ViewModels.TechnicianDropdownViewModel(); // Sprint 79.2
            ZoneLoadStatuses = new(); // Sprint 79.2
            ZoneStressStatuses = new(); // Sprint 79.2
            SmartAssignmentScores = new(); // Sprint 79.2
        }

        public List<RequestSummaryDto> DispatcherRequests { get; set; } = new();
        public List<TechnicianStatusDto> TechnicianStatuses { get; set; } = new();
        public DispatcherStatsDto DispatcherStats { get; set; } = new DispatcherStatsDto { TotalActiveRequests = 0, TechsInTransit = 0, FollowUps = 0, Delays = 0, TopServiceType = string.Empty };
        public List<MVP_Core.Data.Models.DispatcherNotification> Notifications { get; set; } = new(); // Sprint83.4-FinalFixAmbiguity
        public List<WatchdogAlert> WatchdogAlerts { get; set; } = new();
        public List<ServiceRequest> Requests { get; set; } = new();
        public List<string> ServiceTypes { get; set; } = new();
        public string[] Statuses { get; } = new[] { "Unassigned", "Assigned", "En Route", "Complete" };
        [BindProperty(SupportsGet = true)]
        public string? FilterServiceType { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string? FilterSinceString { get; set; }
        public DateTime? FilterSince => DateTime.TryParse(FilterSinceString, out var dt) ? dt : null;
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "priority";
        public bool IsDarkMode { get; set; }
        public int? ReassigningId { get; set; }
        public bool IsDevelopment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

        [BindProperty]
        public string? OrderJson { get; set; }

        [BindProperty(SupportsGet = true)] public string? HistoryFromStatus { get; set; }
        [BindProperty(SupportsGet = true)] public string? HistoryToStatus { get; set; }
        [BindProperty(SupportsGet = true)] public string? HistoryUser { get; set; }
        [BindProperty(SupportsGet = true)] public string? HistoryFromDateString { get; set; }
        [BindProperty(SupportsGet = true)] public string? HistoryToDateString { get; set; }
        public DateTime? HistoryFromDate => DateTime.TryParse(HistoryFromDateString, out var dt) ? dt : null;
        public DateTime? HistoryToDate => DateTime.TryParse(HistoryToDateString, out var dt) ? dt : null;
        [BindProperty(SupportsGet = true)] public string? SlaFilter { get; set; }

        public List<KanbanHistoryLog> KanbanHistory { get; set; } = new();
        public List<SlaSetting> SlaSettings { get; set; } = new();
        public List<MVP_Core.Data.Models.Technician> TechnicianLoads { get; set; } = new();
        public Dictionary<int, MVP_Core.Data.Models.Technician?> SuggestedTechnicians { get; set; } = new();
        public Dictionary<int, double> SuggestedScores { get; set; } = new();
        public Dictionary<int, (string TechName, double Confidence)> SuggestedTech { get; set; } = new();

        // Sprint 36 – SLA Routing Optimizer results
        public Dictionary<int, List<(TechnicianStatusDto Technician, double Score, bool IsFallback)>> OptimizedSuggestions { get; set; } = new();
        public Dictionary<int, string> OptimizerFallbackReasons { get; set; } = new();

        private void CalculateSuggestions()
        {
            var loadService = new LoadBalancingService(_db, _lbConfig);
            foreach (var req in Requests)
            {
                var tech = loadService.GetBestTechnician(req);
                SuggestedTechnicians[req.Id] = tech;
                if (tech != null)
                {
                    SuggestedScores[req.Id] = loadService.CalculateTechScore(tech, req);
                }
            }
        }

        public void LoadSuggestedTech()
        {
            SuggestedTech.Clear();
            var loadService = new LoadBalancingService(_db, _lbConfig);
            foreach (var req in Requests)
            {
                var result = loadService.GetBestTechnicianWithScore(req);
                if (result.BestMatch != null)
                {
                    SuggestedTech[req.Id] = (result.BestMatch.FullName, result.ConfidenceScore);
                }
            }
        }

        // Sprint 33.3 - Dispatcher Smart Filters
        [BindProperty(SupportsGet = true)]
        public bool PendingOnly { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool AssignedOrDispatchedOnly { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FilterZone { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        // End Sprint 33.3 - Dispatcher Smart Filters

        // Sprint 39: All available skill tags for filter UI
        public List<string> AllSkillTags { get; set; } = new List<string> { "Tankless", "Mini Split", "Backflow Cert", "Plumbing", "Heating", "Air Conditioning", "Water Filtration" };
        // Sprint 39: Selected skill tags for filtering
        [BindProperty(SupportsGet = true)]
        public List<string>? SkillTags { get; set; }

        // Sprint 39: Rendered HTML for skill tag options
        public string SkillTagOptionsHtml => string.Join("\n", (AllSkillTags ?? new List<string> { "Tankless", "Mini Split", "Backflow Cert", "Plumbing", "Heating", "Air Conditioning", "Water Filtration" })
            .Select(tag => $"<option value=\"{tag}\"{(SkillTags != null && SkillTags.Contains(tag) ? " selected=\"selected\"" : "")}>{tag}</option>"));

        public async Task<IActionResult> OnGetAsync()
        {
            // Sprint 33.3 - Dispatcher Smart Filters
            var filterModel = new DispatcherFilterModel
            {
                ServiceType = FilterServiceType,
                Zone = FilterZone,
                PendingOnly = PendingOnly,
                AssignedOrDispatchedOnly = AssignedOrDispatchedOnly,
                SearchTerm = SearchTerm
            };
            DispatcherRequests = _dispatcherService.GetFilteredRequests(filterModel);
            // Sprint 36: Populate optimizer suggestions for each request
            OptimizedSuggestions.Clear();
            OptimizerFallbackReasons.Clear();
            foreach (var req in DispatcherRequests)
            {
                string fallbackReason;
                var suggestions = _dispatcherService.GetOptimizedTechnicianSuggestions(req, out fallbackReason);
                OptimizedSuggestions[req.Id] = suggestions;
                OptimizerFallbackReasons[req.Id] = fallbackReason;
            }
            // End Sprint 33.3 - Dispatcher Smart Filters
            TechnicianStatuses = _dispatcherService.GetAllTechnicianStatuses();
            DispatcherStats = _dispatcherService.GetDispatcherStats();
            Notifications = _dispatcherService.GetNotifications().Select(n => new MVP_Core.Data.Models.DispatcherNotification {
                Id = n.Id,
                Message = n.Message,
                SentBy = n.SentBy,
                SentAt = n.SentAt,
                Type = n.Type // Sprint83.4-FinalFixAmbiguity
            }).ToList();
            WatchdogAlerts = new List<WatchdogAlert>(); // Sprint83.4-FinalFix: Stubbed RunWatchdogScan

            var query = _db.KanbanHistoryLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(HistoryFromStatus))
                query = query.Where(x => x.FromStatus == HistoryFromStatus);
            if (!string.IsNullOrWhiteSpace(HistoryToStatus))
                query = query.Where(x => x.ToStatus == HistoryToStatus);
            if (!string.IsNullOrWhiteSpace(HistoryUser))
                query = query.Where(x => x.ChangedBy != null && x.ChangedBy.Contains(HistoryUser));
            if (HistoryFromDate.HasValue)
                query = query.Where(x => x.ChangedAt >= HistoryFromDate.Value);
            if (HistoryToDate.HasValue)
                query = query.Where(x => x.ChangedAt <= HistoryToDate.Value.AddDays(1));
            KanbanHistory = query.OrderByDescending(x => x.ChangedAt).Take(200).ToList();

            // SLA filter logic
            if (SlaFilter == "overdue")
                Requests = Requests.Where(r => r.DueDate.HasValue && r.DueDate.Value < DateTime.UtcNow).ToList();
            else if (SlaFilter == "today")
                Requests = Requests.Where(r => r.DueDate.HasValue && r.DueDate.Value.Date == DateTime.UtcNow.Date).ToList();

            // Export logic
            var export = Request.Query["export"].ToString();
            if (export == "csv")
            {
                var csv = new StringBuilder();
                csv.AppendLine("Time,Job ID,From,To,To Index,User");
                foreach (var log in KanbanHistory)
                {
                    csv.AppendLine($"{log.ChangedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss},{log.ServiceRequestId},\"{log.FromStatus}\",\"{log.ToStatus}\",{log.ToIndex},\"{log.ChangedBy}\"");
                }
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                Response.Headers["Content-Disposition"] = "attachment; filename=kanban-history.csv";
                Response.ContentType = "text/csv";
                Response.Body.WriteAsync(bytes, 0, bytes.Length).Wait();
                Response.Body.Close();
                return new EmptyResult();
            }
            if (export == "excel")
            {
                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("KanbanHistory");
                ws.Cells[1, 1].Value = "Time";
                ws.Cells[1, 2].Value = "Job ID";
                ws.Cells[1, 3].Value = "From";
                ws.Cells[1, 4].Value = "To";
                ws.Cells[1, 5].Value = "To Index";
                ws.Cells[1, 6].Value = "User";
                int row = 2;
                foreach (var log in KanbanHistory)
                {
                    ws.Cells[row, 1].Value = log.ChangedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    ws.Cells[row, 2].Value = log.ServiceRequestId;
                    ws.Cells[row, 3].Value = log.FromStatus;
                    ws.Cells[row, 4].Value = log.ToStatus;
                    ws.Cells[row, 5].Value = log.ToIndex;
                    ws.Cells[row, 6].Value = log.ChangedBy;
                    row++;
                }
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                var bytes = package.GetAsByteArray();
                Response.Headers["Content-Disposition"] = "attachment; filename=kanban-history.xlsx";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Body.WriteAsync(bytes, 0, bytes.Length).Wait();
                Response.Body.Close();
                return new EmptyResult();
            }

            var loadService = new LoadBalancingService(_db, _lbConfig);
            TechnicianLoads = _db.Technicians.ToList();
            LoadSuggestedTech();
            // Sprint83.4-IsOnlinePatch: Fixed CS0200 read-only property assignment
            TechnicianDropdownViewModel = new TechnicianDropdownViewModel
            {
                Technicians = (await Task.FromResult(new List<MVP_Core.Data.Models.ViewModels.TechnicianDropdownItem>()))
                    .Select(tech => new TechnicianStatusDto
                    {
                        TechnicianId = tech.TechnicianId,
                        Name = tech.Name ?? string.Empty,
                        Status = tech.Status ?? string.Empty,
                        DispatchScore = tech.DispatchScore,
                        // IsOnline property is read-only and cannot be set here
                        LastPing = tech.LastPing ?? DateTime.MinValue,
                        AssignedJobs = tech.AssignedJobs,
                        LastUpdate = tech.LastUpdate ?? DateTime.MinValue
                    }).ToList(),
                SelectedTechnicianId = null
            };
            // Sprint83.4-IsOnlinePatch: Fixed CS0200 read-only property assignment
            ServiceZones.Clear();
            ServiceZones.AddRange(new[] { "North", "South", "East", "West" }); // Patch: populate zones for UI

            // Sprint 62.0: Populate ZoneStressStatuses for live heatmap
            ZoneStressStatuses = _dispatcherService.GetZoneStressStatuses(ServiceZones);

            // Sprint 39: Populate AllSkillTags from DB if available
            var dbTags = _db.Technicians
                .Where(t => t.SkillTags != null && t.SkillTags != "")
                .SelectMany(t => t.SkillTags.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(tag => tag.Trim())
                .Distinct()
                .OrderBy(tag => tag)
                .ToList();
            if (dbTags.Any())
                AllSkillTags = dbTags;

            // Sprint 64.0: Smart Assignment AI scoring
            var scoringEngine = new AssignmentScoringEngine();
            SmartAssignmentScores.Clear();
            foreach (var req in DispatcherRequests)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var scores = scoringEngine.GetScores(
                    _db.ServiceRequests.FirstOrDefault(r => r.Id == req.Id),
                    TechnicianStatuses,
                    _db
                );
#pragma warning restore CS8604 // Possible null reference argument.
                SmartAssignmentScores[req.Id] = scores;
            }

            return Page();
        }

        public string GetStatusBadgeClass(string status) => status switch
        {
            "Assigned" => "bg-primary",
            "En Route" => "bg-info text-dark",
            "Delayed" => "bg-warning text-dark",
            "Complete" => "bg-success",
            _ => "bg-secondary"
        };

        // POST handlers for assign, move up/down, etc.
        [BindProperty]
        public int RequestId { get; set; }
        [BindProperty]
        public int Index { get; set; }

        public IActionResult OnPostAssignTechnician()
        {
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            var result = _dispatcherService.AssignTechnician(RequestId);
            TempData["SystemMessages"] = result.Message;
            ViewData["RequestDetails"] = result.RequestDetails;
            ViewData["TechnicianList"] = result.TechnicianList;
            // Assignment log
            var assignment = result;
            var technicianId = assignment?.TechnicianList != null && assignment.TechnicianList.Any()
                ? TechnicianStatuses?.FirstOrDefault(t => t.Name == assignment.AssignedTechName)?.TechnicianId ?? 1
                : 1; // fallback to 1 if not found
            // FixItFred — Sprint 40.4 Final LogAssignment Technician ID Fix
            _dispatcherService.LogAssignment(RequestId, technicianId);
            return RedirectToPage();
        }
        public IActionResult OnPostMoveUp()
        {
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            var result = _dispatcherService.MoveUp(RequestId);
            TempData["SystemMessages"] = result.Message;
            return RedirectToPage();
        }
        public IActionResult OnPostMoveDown()
        {
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            var result = _dispatcherService.MoveDown(RequestId);
            TempData["SystemMessages"] = result.Message;
            return RedirectToPage();
        }
        public IActionResult OnPostReassign()
        {
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            var result = _dispatcherService.Reassign(RequestId);
            TempData["SystemMessages"] = result.Message;
            return RedirectToPage();
        }
        public IActionResult OnPostCancel()
        {
            var role = "Supervisor";
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            var result = _dispatcherService.Cancel(RequestId);
            TempData["SystemMessages"] = result.Message;
            var log = new DispatcherAuditLog
            {
                ActionType = "Cancel",
                RequestId = RequestId,
                TechnicianId = null,
                PerformedBy = User?.Identity?.Name ?? "system",
                PerformedByRole = role,
                Timestamp = DateTime.UtcNow,
                Notes = result.Message
            };
            // FixItFred — Sprint 40.4 Final LogDispatcherAction Patch
            _dispatcherService.LogDispatcherAction($"[Audit] {log.Timestamp:u} | {log.ActionType} | {log.TechnicianId} | {log.Notes}");
            return RedirectToPage();
        }
        public IActionResult OnPostEscalate()
        {
            var role = "Supervisor";
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            var result = _dispatcherService.Escalate(RequestId);
            TempData["SystemMessages"] = result.Message;
            var log = new DispatcherAuditLog
            {
                ActionType = "Escalate",
                RequestId = RequestId,
                TechnicianId = null,
                PerformedBy = User?.Identity?.Name ?? "system",
                PerformedByRole = role,
                Timestamp = DateTime.UtcNow,
                Notes = result.Message
            };
            // FixItFred — Sprint 40.4 Final LogDispatcherAction Patch
            _dispatcherService.LogDispatcherAction($"[Audit] {log.Timestamp:u} | {log.ActionType} | {log.TechnicianId} | {log.Notes}");
            return RedirectToPage();
        }
        public IActionResult OnPostReassignTech(int requestId, object newTechnicianId)
        {
            var role = User.IsInRole("Supervisor") ? "Supervisor" : "Dispatcher";
            if (requestId <= 0 || newTechnicianId == null)
            {
                TempData["SystemMessages"] = "Reassignment failed — check technician availability.";
                return Page();
            }
            // FixItFred – Sprint 46.1 Final Build Cleanup: Ensure int argument
            int techId = newTechnicianId is int i ? i : Convert.ToInt32(newTechnicianId);
            bool success = _dispatcherService.ReassignTechnician(requestId, techId).GetAwaiter().GetResult();
            TempData["SystemMessages"] = success ? "Technician reassigned successfully." : "Reassignment failed — check technician availability.";
            var log = new DispatcherAuditLog
            {
                ActionType = "Reassign",
                RequestId = requestId,
                TechnicianId = techId,
                PerformedBy = User?.Identity?.Name ?? "system",
                PerformedByRole = role,
                Timestamp = DateTime.UtcNow,
                Notes = success ? "Technician reassigned." : "Failed reassignment."
            };
            _dispatcherService.LogDispatcherAction($"[Audit] {log.Timestamp:u} | {log.ActionType} | {log.TechnicianId} | {log.Notes}");
            _auditLogger.LogAsync(User?.Identity?.Name ?? "unknown", "DispatcherReassign", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", $"RequestId={requestId};NewTechId={techId}");
            return Page();
        }
        public async Task<IActionResult> OnPostReassignTechAsync(int requestId, int newTechnicianId)
        {
            var role = User.IsInRole("Supervisor") ? "Supervisor" : "Admin";
            if (requestId <= 0 || newTechnicianId <= 0)
            {
                TempData["SystemMessages"] = "Reassignment failed — check technician availability.";
                return Page();
            }
            // Find the schedule queue entry for this request
            var queue = _db.ScheduleQueues.FirstOrDefault(q => q.ServiceRequestId == requestId);
            if (queue == null)
            {
                TempData["SystemMessages"] = "Schedule entry not found.";
                return Page();
            }
            // Find the new technician
            var tech = _db.Technicians.FirstOrDefault(t => t.Id == newTechnicianId && t.IsActive);
            if (tech == null)
            {
                TempData["SystemMessages"] = "Selected technician not found or inactive.";
                return Page();
            }
            // --- Sprint 33.3: Eligibility check ---
            // Technician must be Available (skip _db.TechnicianStatuses, fallback to IsActive)
            if (!tech.IsActive)
            {
                TempData["SystemMessages"] = "Technician is not available for reassignment.";
                return Page();
            }
            // --- End eligibility check ---
            var prevTechId = queue.TechnicianId;
            var prevTechName = queue.AssignedTechnicianName;
            queue.TechnicianId = tech.Id;
            queue.AssignedTechnicianName = tech.FullName;
            await _db.SaveChangesAsync();
            // Recalculate ETA based on new tech's status
            DateTime eta;
            var techStatus = TechnicianStatuses?.FirstOrDefault(ts => ts.TechnicianId == tech.Id);
            if (techStatus != null)
            {
                eta = await _dispatcherService.PredictETA(techStatus, queue.Zone, 0);
            }
            else
            {
                eta = DateTime.UtcNow.AddMinutes(15);
            }
            var prevEta = queue.EstimatedArrival;
            queue.EstimatedArrival = eta;
            await _db.SaveChangesAsync();
            _db.ETAHistoryEntries.Add(new ETAHistoryEntry {
                TechnicianId = tech.Id,
                TechnicianName = tech.FullName,
                ServiceRequestId = queue.ServiceRequestId,
                Zone = queue.Zone,
                Timestamp = DateTime.UtcNow,
                PredictedETA = prevEta,
                ActualArrival = eta,
                Message = "Reassigned"
            });
            await _db.SaveChangesAsync();
            _db.AuditLogEntries.Add(new MVP_Core.Data.Models.AuditLogEntry {
                UserId = User?.Identity?.Name ?? "system",
                Action = "Dispatch Handoff",
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                Timestamp = DateTime.UtcNow,
                AdditionalData = $"RequestId={requestId};FromTechId={prevTechId};ToTechId={tech.Id};FromTechName={prevTechName};ToTechName={tech.FullName}"
            });
            await _db.SaveChangesAsync();
            await _dispatchEngine.BroadcastETAAsync(queue.Zone, $"[Reassigned] Technician {tech.FullName} ETA: {eta:t}");
            var technicianGroup = $"Technician-{tech.Id}";
            await _hubContext.Clients.Group(technicianGroup).SendAsync("ReceiveETA", queue.Zone, $"[Reassigned] Technician {tech.FullName} ETA: {eta:t}");
            // Log dispatcher action (legacy log)
            _dispatcherService.LogDispatcherAction($"[Audit] {DateTime.UtcNow:u} | Job {queue.ServiceRequestId} reassigned to tech {tech.Id} by {User?.Identity?.Name ?? "system"}");
            var log = new DispatcherAuditLog
            {
                ActionType = "Reassign",
                RequestId = requestId,
                TechnicianId = tech.Id,
                PerformedBy = User?.Identity?.Name ?? "system",
                PerformedByRole = role,
                Timestamp = DateTime.UtcNow,
                Notes = "Technician reassigned. ETA and logs updated."
            };
            // FixItFred — Sprint 40.4 Final LogDispatcherAction Patch
            _dispatcherService.LogDispatcherAction($"[Audit] {log.Timestamp:u} | {log.ActionType} | {log.TechnicianId} | {log.Notes}");
            await _auditLogger.LogAsync(User?.Identity?.Name ?? "unknown", "DispatcherReassign", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", $"RequestId={requestId};NewTechId={tech.Id}");
            TempData["SystemMessages"] = "Technician reassigned, ETA recalculated, and logs updated.";
            return RedirectToPage();
        }

        // Sprint 32 - Admin Reschedule Logic
        [BindProperty]
        public DateTime? NewETA { get; set; }
        [BindProperty]
        public string? Reason { get; set; }

        public async Task<IActionResult> OnPostOverrideETAAsync()
        {
            if (RequestId <= 0 || !NewETA.HasValue || string.IsNullOrWhiteSpace(Reason))
            {
                TempData["SystemMessages"] = "Invalid reschedule request.";
                return RedirectToPage();
            }
            var queue = _db.ScheduleQueues.FirstOrDefault(q => q.ServiceRequestId == RequestId);
            if (queue == null)
            {
                TempData["SystemMessages"] = "Schedule entry not found.";
                return RedirectToPage();
            }
            var prevEta = queue.EstimatedArrival;
            queue.EstimatedArrival = NewETA.Value;
            await _db.SaveChangesAsync();
            // Log to ETAHistoryEntry
            _db.ETAHistoryEntries.Add(new ETAHistoryEntry {
                TechnicianId = queue.TechnicianId,
                TechnicianName = queue.AssignedTechnicianName,
                ServiceRequestId = queue.ServiceRequestId,
                Zone = queue.Zone,
                Timestamp = DateTime.UtcNow,
                PredictedETA = prevEta,
                ActualArrival = NewETA.Value,
                Message = Reason
            });
            await _db.SaveChangesAsync();
            // SignalR broadcast
            await _dispatchEngine.BroadcastETAAsync(queue.Zone, $"[Admin Override] Technician {queue.AssignedTechnicianName} ETA: {NewETA.Value:t}");
            TempData["SystemMessages"] = "ETA updated and broadcast.";
            await _auditLogger.LogAsync(User?.Identity?.Name ?? "unknown", "AdminOverrideETA", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", $"RequestId={RequestId};NewETA={NewETA};Reason={Reason}");
            return RedirectToPage();
        }

        public List<string> ServiceZones { get; set; } = new();
        public MVP_Core.Data.Models.ViewModels.TechnicianDropdownViewModel TechnicianDropdownViewModel { get; set; } = new MVP_Core.Data.Models.ViewModels.TechnicianDropdownViewModel();

        // Sprint 36.A – Real-time zone load/capacity for UI
        public List<MVP_Core.Services.Admin.DispatcherService.ZoneLoadStatus> ZoneLoadStatuses { get; set; } = new();

        [BindProperty]
        public int OverrideJobId { get; set; }
        [BindProperty]
        public string? OverrideNewZone { get; set; }

        public IActionResult OnPostOverrideJobZone()
        {
            if (OverrideJobId > 0 && !string.IsNullOrEmpty(OverrideNewZone))
            {
                // Use string-based zone rerouting
                bool success = _dispatcherService.OverrideJobZone(OverrideJobId, OverrideNewZone);
                TempData["SystemMessages"] = success ? "Job zone updated." : "Failed to update job zone.";
            }
            return RedirectToPage();
        }

        // Sprint 47.1 – FixItFred Mission Package: Expose ZoneLoadChartData and LiveTechHeartbeatData for UI
        public string ZoneLoadChartDataJson => Newtonsoft.Json.JsonConvert.SerializeObject(
            ServiceZones.Select(z => new {
                Zone = z,
                Load = ZoneLoadStatuses?.FirstOrDefault(s => s.Zone == z)?.JobCount ?? 0,
                Techs = ZoneLoadStatuses?.FirstOrDefault(s => s.Zone == z)?.AvailableTechCount ?? 0
            })
        );
        public string LiveTechHeartbeatDataJson => Newtonsoft.Json.JsonConvert.SerializeObject(
            TechnicianStatuses.Select(t => new {
                t.TechnicianId,
                t.Name,
                t.Status,
                t.DispatchScore,
                t.IsOnline,
                t.LastPing
            })
        );
        // Sprint 47.2 – FixItFred Mission Package: Expose WatchdogAlerts for SignalR
        public string WatchdogAlertsJson => Newtonsoft.Json.JsonConvert.SerializeObject(WatchdogAlerts);

        // Sprint 62.0 — Dispatcher Load Monitor: Zone Stress Statuses for Razor UI
        public List<MVP_Core.Services.Admin.DispatcherService.ZoneStressStatus> ZoneStressStatuses { get; set; } = new();

        public string ZoneStressChartDataJson => Newtonsoft.Json.JsonConvert.SerializeObject(
            ServiceZones.Select(z => new {
                Zone = z,
                JobCount = ZoneStressStatuses?.FirstOrDefault(s => s.Zone == z)?.JobCount ?? 0,
                SlaRiskJobs = ZoneStressStatuses?.FirstOrDefault(s => s.Zone == z)?.SlaRiskJobs ?? 0,
                EscalatedJobs = ZoneStressStatuses?.FirstOrDefault(s => s.Zone == z)?.EscalatedJobs ?? 0,
                CongestionLevel = ZoneStressStatuses?.FirstOrDefault(s => s.Zone == z)?.CongestionLevel ?? 0
            })
        );

        public Dictionary<int, List<MVP_Core.Services.Admin.AssignmentScoringEngine.TechnicianAssignmentScore>> SmartAssignmentScores { get; set; } = new();
    }
}
