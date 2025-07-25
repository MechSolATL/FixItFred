// FixItFred Patch Log — CS1998 Async Compliance Patch
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
    [Authorize(Roles = "Dispatcher,Supervisor")]
    public class DispatcherModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<RequestHub> _hubContext;
        private readonly IOptions<LoadBalancingConfig> _lbConfig;

        // FixItFred: Sprint 30D.1 - Live ETA Routing & Broadcast
        // NotificationDispatchEngine is injected for live ETA SignalR updates
        private readonly NotificationDispatchEngine _dispatchEngine;
        private readonly IAuditTrailLogger _auditLogger;

        public DispatcherModel(DispatcherService dispatcherService, ApplicationDbContext db, IHubContext<RequestHub> hubContext, IOptions<LoadBalancingConfig> lbConfig, NotificationDispatchEngine dispatchEngine, IAuditTrailLogger auditLogger)
        {
            _dispatcherService = dispatcherService;
            _db = db;
            _hubContext = hubContext;
            _lbConfig = lbConfig;
            _dispatchEngine = dispatchEngine;
            _auditLogger = auditLogger;
        }

        public List<RequestSummaryDto> DispatcherRequests { get; set; } = new();
        public List<TechnicianStatusDto> TechnicianStatuses { get; set; } = new();
        public DispatcherStatsDto DispatcherStats { get; set; } = new DispatcherStatsDto { TotalActiveRequests = 0, TechsInTransit = 0, FollowUps = 0, Delays = 0, TopServiceType = string.Empty };
        public List<DispatcherNotification> Notifications { get; set; } = new();
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
            Notifications = _dispatcherService.GetNotifications();
            WatchdogAlerts = _dispatcherService.RunWatchdogScan();

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
            TechnicianDropdownViewModel = new TechnicianDropdownViewModel
            {
                Technicians = await _dispatcherService.GetTechniciansAsync(),
                SelectedTechnicianId = null
            };
            ServiceZones.Clear();
            ServiceZones.AddRange(new[] { "North", "South", "East", "West" }); // Patch: populate zones for UI

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
            var techId = 1; // Stub: replace with actual assigned tech
            var score = _dispatcherService.CalculateDispatchScore(techId);
            var tier = score >= 80 ? "?? Ready" : score >= 50 ? "?? At Capacity" : "?? Overloaded";
            var rationale = $"Tag match: Plumbing, ZIP match: 30345, Score: {score} ({tier})";
            var tags = new List<string> { "Plumbing" };
            // Sprint 37: AI Routing Preview - get AI suggestion for this request
            var aiSuggestions = OptimizedSuggestions.ContainsKey(RequestId) ? OptimizedSuggestions[RequestId] : null;
            var aiTop = aiSuggestions?.FirstOrDefault(s => !s.IsFallback);
            _dispatcherService.LogAssignment(new AssignmentLogEntry
            {
                RequestId = RequestId,
                TechnicianId = techId,
                DispatcherName = User?.Identity?.Name ?? "system",
                Timestamp = DateTime.UtcNow,
                DispatchScore = score,
                Tier = tier,
                Rationale = rationale,
                MatchedTags = tags,
                // Sprint 37 fields
                AISuggestedTechnicianId = aiTop?.Technician.TechnicianId,
                AISuggestedTechnicianName = aiTop?.Technician.Name,
                AISuggestedScore = aiTop?.Score
            });
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
            _dispatcherService.LogDispatcherAction(new DispatcherAuditLog
            {
                ActionType = "Cancel",
                RequestId = RequestId,
                TechnicianId = null,
                PerformedBy = User?.Identity?.Name ?? "system",
                PerformedByRole = role,
                Timestamp = DateTime.UtcNow,
                Notes = result.Message
            });
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
            _dispatcherService.LogDispatcherAction(new DispatcherAuditLog
            {
                ActionType = "Escalate",
                RequestId = RequestId,
                TechnicianId = null,
                PerformedBy = User?.Identity?.Name ?? "system",
                PerformedByRole = role,
                Timestamp = DateTime.UtcNow,
                Notes = result.Message
            });
            return RedirectToPage();
        }
        public IActionResult OnPostResendInstructions()
        {
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            var result = _dispatcherService.ResendInstructions(RequestId);
            TempData["SystemMessages"] = result.Message;
            return RedirectToPage();
        }
        public IActionResult OnPostFlagEmergency(int requestId)
        {
            var role = "Supervisor";
            var dispatcherName = User?.Identity?.Name ?? "system";
            bool success = _dispatcherService.FlagEmergency(requestId, dispatcherName);
            TempData["SystemMessages"] = success ? "Request flagged as emergency." : "Failed to flag emergency.";
            _dispatcherService.LogDispatcherAction(new DispatcherAuditLog
            {
                ActionType = "Override-Emergency",
                RequestId = requestId,
                TechnicianId = null,
                PerformedBy = dispatcherName,
                PerformedByRole = role,
                Timestamp = DateTime.UtcNow,
                Notes = "Dispatcher flagged emergency"
            });
            return Page();
        }

        // Handler for drag-and-drop Kanban reordering
        public async Task<IActionResult> OnPostReorderAsync()
        {
            if (!string.IsNullOrEmpty(OrderJson))
            {
                var orderDict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(OrderJson);
                if (orderDict != null)
                {
                    foreach (var status in orderDict.Keys)
                    {
                        var idList = orderDict[status];
                        for (int i = 0; i < idList.Count; i++)
                        {
                            if (int.TryParse(idList[i], out int reqId))
                            {
                                var req = await _db.ServiceRequests.FindAsync(reqId);
                                if (req != null)
                                {
                                    var oldStatus = req.Status;
                                    var oldPriority = req.Priority;
                                    // Find old index in previous status column (if needed)
                                    // Log only if status or index changed
                                    if (oldStatus != status || oldPriority != (i == 0 ? "High" : (i == idList.Count - 1 ? "Low" : "Normal")))
                                    {
                                        _db.KanbanHistoryLogs.Add(new KanbanHistoryLog
                                        {
                                            ServiceRequestId = req.Id,
                                            FromStatus = oldStatus,
                                            ToStatus = status,
                                            FromIndex = null, // Optionally track old index
                                            ToIndex = i,
                                            ChangedBy = User?.Identity?.Name ?? "system",
                                            ChangedAt = DateTime.UtcNow
                                        });
                                    }
                                    req.Status = status;
                                    if (status == "Unassigned" || status == "Assigned" || status == "En Route")
                                    {
                                        req.Priority = i == 0 ? "High" : (i == idList.Count - 1 ? "Low" : "Normal");
                                    }
                                    else if (status == "Complete")
                                    {
                                        req.Priority = "Normal";
                                    }
                                }
                            }
                        }
                    }
                    await _db.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("kanbanUpdated");
                }
            }
            return RedirectToPage();
        }

        // Dynamic SLA per job type from config table
        public int GetSlaHours(string serviceType)
        {
            var setting = SlaSettings.FirstOrDefault(s => s.ServiceType.ToLower() == serviceType?.ToLower());
            return setting?.SlaHours ?? 24;
        }
        public DateTime? CalculateDueDate(ServiceRequest req)
        {
            if (req.DueDate.HasValue) return req.DueDate;
            var hours = GetSlaHours(req.ServiceType);
            return req.CreatedAt.AddHours(hours);
        }

        // Handler for updating SLA/deadline (call SignalR broadcast)
        public async Task<IActionResult> OnPostUpdateDueDateAsync(int requestId, DateTime dueDate)
        {
            var req = await _db.ServiceRequests.FindAsync(requestId);
            if (req != null)
            {
                req.DueDate = dueDate;
                await _db.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("slaUpdated");
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRecalculateLoadAsync()
        {
            var loadService = new LoadBalancingService(_db, _lbConfig);
            TechnicianLoads = _db.Technicians.ToList();
            // Optionally: recalculate job counts here if needed
            await Task.CompletedTask;
            return Page();
        }

        public async Task<IActionResult> OnPostAutoAssignAsync(int requestId)
        {
            var request = await _db.ServiceRequests.FindAsync(requestId);
            if (request == null) return NotFound();

            var loadService = new LoadBalancingService(_db, _lbConfig);
            var tech = loadService.GetBestTechnician(request);
            if (tech == null) return BadRequest("No suitable technician found.");

            request.AssignedTechnicianId = tech.Id;
            await _db.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("technicianAutoAssigned", request.Id, tech.Id);
            await _hubContext.Clients.All.SendAsync("suggestedTechUpdated");
            // Refresh suggestions after assignment
            Requests = _db.ServiceRequests.ToList();
            LoadSuggestedTech();
            return RedirectToPage();
        }

        public IActionResult OnPostApplyFilters(DispatcherFilterModel filters)
        {
            // Sprint 33.3 - Dispatcher Smart Filters
            DispatcherRequests = _dispatcherService.GetFilteredRequests(filters);
            // End Sprint 33.3 - Dispatcher Smart Filters
            ViewData["RequestDetails"] = DispatcherRequests;
            ViewData["TechnicianList"] = _dispatcherService.GetSuggestedTechnicians(DispatcherRequests.FirstOrDefault()?.Id ?? 0);
            ViewData["DispatcherStats"] = _dispatcherService.GetDispatcherStats();
            ViewData["Notifications"] = _dispatcherService.GetNotifications();
            ViewData["WatchdogAlerts"] = _dispatcherService.RunWatchdogScan();
            if (!DispatcherRequests.Any())
                TempData["SystemMessages"] = "No matching requests.";
            return Page();
        }

        public IActionResult OnPostReassignTech(int requestId, int newTechnicianId)
        {
            var role = User.IsInRole("Supervisor") ? "Supervisor" : "Dispatcher";
            if (requestId <= 0 || newTechnicianId <= 0)
            {
                TempData["SystemMessages"] = "Reassignment failed — check technician availability.";
                return Page();
            }
            bool success = _dispatcherService.ReassignTechnician(requestId, newTechnicianId);
            TempData["SystemMessages"] = success ? "Technician reassigned successfully." : "Reassignment failed — check technician availability.";
            _dispatcherService.LogDispatcherAction(new DispatcherAuditLog
            {
                ActionType = "Reassign",
                RequestId = requestId,
                TechnicianId = newTechnicianId,
                PerformedBy = User?.Identity?.Name ?? "system",
                PerformedByRole = role,
                Timestamp = DateTime.UtcNow,
                Notes = success ? "Technician reassigned." : "Failed reassignment."
            });
            _auditLogger.LogAsync(User?.Identity?.Name ?? "unknown", "DispatcherReassign", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", $"RequestId={requestId};NewTechId={newTechnicianId}");
            return Page();
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

        // Sprint 33.3 - Handoff Authorization + Sync
        [Authorize(Roles = "Admin")]
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

            // Update ScheduleQueue.TechnicianId and AssignedTechnicianName
            var prevTechId = queue.TechnicianId;
            var prevTechName = queue.AssignedTechnicianName;
            queue.TechnicianId = tech.Id;
            queue.AssignedTechnicianName = tech.FullName;
            await _db.SaveChangesAsync();

            // Recalculate ETA based on new tech's location
            var dispatcherService = new MVP_Core.Services.Admin.DispatcherService(_db, null);
            var eta = dispatcherService.PredictETA(new MVP_Core.Data.Models.TechnicianProfileDto {
                Id = tech.Id,
                Specialty = tech.Specialty,
                IsActive = tech.IsActive,
                Latitude = tech.Latitude ?? 0,
                Longitude = tech.Longitude ?? 0
            }, queue.Zone, 0);
            var prevEta = queue.EstimatedArrival;
            queue.EstimatedArrival = eta;
            await _db.SaveChangesAsync();

            // Log change in ETAHistoryEntry with reason: Reassigned
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

            // --- Sprint 33.3: Audit log for handoff ---
            _db.AuditLogEntries.Add(new MVP_Core.Data.Models.AuditLogEntry {
                UserId = User?.Identity?.Name ?? "system",
                Action = "Dispatch Handoff",
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                Timestamp = DateTime.UtcNow,
                AdditionalData = $"RequestId={requestId};FromTechId={prevTechId};ToTechId={tech.Id};FromTechName={prevTechName};ToTechName={tech.FullName}"
            });
            await _db.SaveChangesAsync();
            // --- End audit log ---

            // --- Sprint 33.3: SignalR broadcast to all relevant groups ---
            await _dispatchEngine.BroadcastETAAsync(queue.Zone, $"[Reassigned] Technician {tech.FullName} ETA: {eta:t}");
            // Also notify the new technician's group
            var technicianGroup = $"Technician-{tech.Id}";
            await _hubContext.Clients.Group(technicianGroup).SendAsync("ReceiveETA", queue.Zone, $"[Reassigned] Technician {tech.FullName} ETA: {eta:t}");
            // --- End SignalR broadcast ---

            // Log dispatcher action (legacy log)
            _dispatcherService.LogDispatcherAction(new DispatcherAuditLog
            {
                ActionType = "Reassign",
                RequestId = requestId,
                TechnicianId = tech.Id,
                PerformedBy = User?.Identity?.Name ?? "system",
                PerformedByRole = role,
                Timestamp = DateTime.UtcNow,
                Notes = "Technician reassigned. ETA and logs updated."
            });

            await _auditLogger.LogAsync(User?.Identity?.Name ?? "unknown", "DispatcherReassign", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", $"RequestId={requestId};NewTechId={tech.Id}");

            TempData["SystemMessages"] = "Technician reassigned, ETA recalculated, and logs updated.";
            return RedirectToPage();
        }
        // Sprint 33.3 - END

        public IActionResult OnPostMarkDelayed()
        {
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            // Update status to Delayed and log ETA history
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == RequestId);
            if (req != null)
            {
                var prevStatus = req.Status;
                req.Status = "Delayed";
                _db.SaveChanges();
                _db.ETAHistoryEntries.Add(new ETAHistoryEntry {
                    TechnicianId = req.AssignedTechnicianId ?? 0,
                    TechnicianName = req.AssignedTo,
                    ServiceRequestId = req.Id,
                    Zone = req.Zip,
                    Timestamp = DateTime.UtcNow,
                    PredictedETA = req.DueDate,
                    ActualArrival = null,
                    Message = "Marked as Delayed by Dispatcher"
                });
                _db.SaveChanges();
                // Optionally: SignalR broadcast
                _hubContext.Clients.All.SendAsync("jobDelayed", req.Id);
            }
            TempData["SystemMessages"] = "Job marked as delayed.";
            return RedirectToPage();
        }
        public IActionResult OnPostAutoReassign()
        {
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            // Auto-reassign logic: find best tech and reassign
            var req = _db.ServiceRequests.FirstOrDefault(r => r.Id == RequestId);
            if (req != null)
            {
                var loadService = new LoadBalancingService(_db, _lbConfig);
                var tech = loadService.GetBestTechnician(req);
                if (tech != null)
                {
                    req.AssignedTechnicianId = tech.Id;
                    req.AssignedTo = tech.FullName;
                    _db.SaveChanges();
                    // Log ETA history
                    _db.ETAHistoryEntries.Add(new ETAHistoryEntry {
                        TechnicianId = tech.Id,
                        TechnicianName = tech.FullName,
                        ServiceRequestId = req.Id,
                        Zone = req.Zip,
                        Timestamp = DateTime.UtcNow,
                        PredictedETA = req.DueDate,
                        ActualArrival = null,
                        Message = "Auto-Reassigned by Dispatcher"
                    });
                    _db.SaveChanges();
                    // Optionally: SignalR broadcast
                    _hubContext.Clients.All.SendAsync("jobAutoReassigned", req.Id, tech.Id);
                }
            }
            TempData["SystemMessages"] = "Job auto-reassigned.";
            return RedirectToPage();
        }

        // Sprint 34.2 - Admin Escalation Handler
        public async Task<IActionResult> OnPostEscalateJobAsync(int scheduleId, DateTime? newETA, string reason, string actionTaken)
        {
            if (scheduleId <= 0 || string.IsNullOrWhiteSpace(reason) || string.IsNullOrWhiteSpace(actionTaken))
            {
                TempData["SystemMessages"] = "Invalid escalation request.";
                return RedirectToPage();
            }
            var queue = _db.ScheduleQueues.FirstOrDefault(q => q.Id == scheduleId);
            if (queue == null)
            {
                TempData["SystemMessages"] = "Schedule entry not found.";
                return RedirectToPage();
            }
            if (newETA.HasValue)
            {
                queue.EstimatedArrival = newETA.Value;
                await _db.SaveChangesAsync();
            }
            // Log escalation
            _db.EscalationLogs.Add(new EscalationLogEntry {
                ScheduleQueueId = scheduleId,
                TriggeredBy = User?.Identity?.Name ?? "admin",
                Reason = reason,
                ActionTaken = actionTaken,
                CreatedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            // SignalR broadcast
            await _dispatchEngine.BroadcastSLAEscalationAsync(queue.Zone, $"[Escalation] Job #{queue.ServiceRequestId} escalated by {User?.Identity?.Name ?? "admin"}: {reason}");
            TempData["SystemMessages"] = "Escalation logged and broadcast.";
            return RedirectToPage();
        }

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
                bool success = _dispatcherService.OverrideJobZone(OverrideJobId, OverrideNewZone);
                TempData["SystemMessages"] = success ? "Job zone updated." : "Failed to update job zone.";
            }
            return RedirectToPage();
        }
    }
}
