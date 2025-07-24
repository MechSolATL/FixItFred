using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using MVP_Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using System.Text;
using System.Globalization;
using OfficeOpenXml;
using Microsoft.Extensions.Options;
using MVP_Core.Services.Config;
using MVP_Core.Services.Admin;
using MVP_Core.Models.Admin;

namespace MVP_Core.Pages.Admin
{
    public class DispatcherModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISeoService _seoService;
        private readonly IDeviceResolver _deviceResolver;
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<RequestHub> _hubContext;
        private readonly IOptions<LoadBalancingConfig> _lbConfig;
        private DispatcherService _dispatcherService;

        public DispatcherModel(IHttpClientFactory httpClientFactory, ISeoService seoService, IDeviceResolver deviceResolver, ApplicationDbContext db, IHubContext<RequestHub> hubContext, IOptions<LoadBalancingConfig> lbConfig)
        {
            _httpClientFactory = httpClientFactory;
            _seoService = seoService;
            _deviceResolver = deviceResolver;
            _db = db;
            _hubContext = hubContext;
            _lbConfig = lbConfig;
        }

        public void InjectDispatcherService(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

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
        public Dictionary<int, string> TechnicianStatuses { get; set; } = new();
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
        public List<Technician> TechnicianLoads { get; set; } = new();
        public Dictionary<int, Technician?> SuggestedTechnicians { get; set; } = new();
        public Dictionary<int, double> SuggestedScores { get; set; } = new();
        public Dictionary<int, (string TechName, double Confidence)> SuggestedTech { get; set; } = new();

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

        public void OnGet()
        {
            var seoTask = _seoService.GetSeoByPageNameAsync("Dispatcher Dashboard");
            seoTask.Wait();
            var seo = seoTask.Result;
            ViewData["Title"] = seo?.Title ?? "Dispatcher Kanban";
            ViewData["MetaDescription"] = seo?.MetaDescription;
            ViewData["Keywords"] = seo?.Keywords;
            ViewData["Robots"] = seo?.Robots;
            ViewData["DeviceType"] = _deviceResolver.GetDeviceType(HttpContext);

            var client = _httpClientFactory.CreateClient();
            var url = $"/api/requests?serviceType={FilterServiceType}&since={(FilterSince?.ToString("o") ?? "")}&sort={SortBy}";
            var requestsTask = client.GetFromJsonAsync<List<ServiceRequest>>(url);
            requestsTask.Wait();
            Requests = requestsTask.Result ?? new();
            var typesTask = client.GetFromJsonAsync<List<string>>("/api/requests/types");
            typesTask.Wait();
            ServiceTypes = typesTask.Result ?? new();
            ViewData["TechnicianStatuses"] = _dispatcherService.GetAllTechnicianStatuses();
            ViewData["DispatcherStats"] = _dispatcherService.GetDispatcherStats();
            ViewData["Notifications"] = _dispatcherService.GetNotifications();
            ViewData["WatchdogAlerts"] = _dispatcherService.RunWatchdogScan();

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
                return;
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
                return;
            }

            var loadService = new LoadBalancingService(_db, _lbConfig);
            TechnicianLoads = _db.Technicians.ToList();
            LoadSuggestedTech();
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
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            var result = _dispatcherService.Cancel(RequestId);
            TempData["SystemMessages"] = result.Message;
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
        public IActionResult OnPostEscalate()
        {
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
                Timestamp = DateTime.UtcNow,
                Notes = result.Message
            });
            return RedirectToPage();
        }
        public IActionResult OnPostShowMap()
        {
            if (RequestId <= 0)
            {
                TempData["SystemMessages"] = "Invalid RequestId.";
                return RedirectToPage();
            }
            var result = _dispatcherService.ShowMap(RequestId);
            TempData["SystemMessages"] = result.Message;
            return RedirectToPage();
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
            var filteredRequests = _dispatcherService.GetFilteredRequests(filters);
            ViewData["RequestDetails"] = filteredRequests;
            ViewData["TechnicianList"] = _dispatcherService.GetSuggestedTechnicians(filteredRequests.FirstOrDefault()?.Id ?? 0);
            ViewData["DispatcherStats"] = _dispatcherService.GetDispatcherStats();
            ViewData["Notifications"] = _dispatcherService.GetNotifications();
            ViewData["WatchdogAlerts"] = _dispatcherService.RunWatchdogScan();
            if (!filteredRequests.Any())
                TempData["SystemMessages"] = "No matching requests.";
            return Page();
        }

        public IActionResult OnPostReassignTech(int requestId, int newTechnicianId)
        {
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
                Timestamp = DateTime.UtcNow,
                Notes = success ? "Technician reassigned." : "Failed reassignment."
            });
            return Page();
        }

        public IActionResult OnPostFlagEmergency(int requestId)
        {
            var dispatcherName = User?.Identity?.Name ?? "system";
            bool success = _dispatcherService.FlagEmergency(requestId, dispatcherName);
            TempData["SystemMessages"] = success ? "Request flagged as emergency." : "Failed to flag emergency.";
            return Page();
        }
    }
}
