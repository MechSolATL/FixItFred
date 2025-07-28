using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
// FixItFred: Remove all ambiguous using for TechnicianAuditService, use fully qualified name
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainTechnicianBehaviorLog = MVP_Core.Models.TechnicianBehaviorLog;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class TechnicianStatusReportModel : PageModel
    {
        private readonly MVP_Core.Services.Admin.TechnicianAuditService _auditService;
        private readonly ApplicationDbContext _db;
        public List<DomainTechnicianBehaviorLog> BehaviorLogs { get; set; } = new();
        public ComplianceStatsViewModel ComplianceStats { get; set; } = new();
        public TechnicianStatusReportModel(MVP_Core.Services.Admin.TechnicianAuditService auditService, ApplicationDbContext db)
        {
            _auditService = auditService;
            _db = db;
        }
        public async Task OnGetAsync()
        {
            BehaviorLogs = await _auditService.GetFlaggedLogsAsync();
            var allLogs = await _auditService.GetAllLogsAsync();
            var techs = _db.Technicians.ToList();
            ComplianceStats.GlobalCompliancePercent = allLogs.Count == 0 ? 100 : 100.0 * allLogs.Count(l => l.Status != "Flagged") / allLogs.Count;
            ComplianceStats.PerTechnician = techs.Select(t => new TechnicianComplianceStat
            {
                Name = t.FullName,
                CompliancePercent = allLogs.Count(l => l.TechnicianId == t.Id) == 0 ? 100 : 100.0 * allLogs.Count(l => l.TechnicianId == t.Id && l.Status != "Flagged") / allLogs.Count(l => l.TechnicianId == t.Id),
                FlaggedCount = allLogs.Count(l => l.TechnicianId == t.Id && l.Status == "Flagged"),
                TimeViolations = allLogs.Count(l => l.TechnicianId == t.Id && l.ViolationType == "Time"),
                LocationViolations = allLogs.Count(l => l.TechnicianId == t.Id && l.ViolationType == "Location"),
                MetadataViolations = allLogs.Count(l => l.TechnicianId == t.Id && l.ViolationType == "Metadata")
            }).ToList();
        }
        public class ComplianceStatsViewModel
        {
            public double GlobalCompliancePercent { get; set; }
            public List<TechnicianComplianceStat> PerTechnician { get; set; } = new();
        }
        public class TechnicianComplianceStat
        {
            public string Name { get; set; } = string.Empty;
            public double CompliancePercent { get; set; }
            public int FlaggedCount { get; set; }
            public int TimeViolations { get; set; }
            public int LocationViolations { get; set; }
            public int MetadataViolations { get; set; }
        }
    }
}
