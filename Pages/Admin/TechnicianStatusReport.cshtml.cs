using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainTechnicianBehaviorLog = Models.TechnicianBehaviorLog;
using Data.Models;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    /// <summary>
    /// Represents the Technician Status Report page model for viewing compliance statistics and flagged logs.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class TechnicianStatusReportModel : PageModel
    {
        private readonly Services.Admin.TechnicianAuditService _auditService;
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// The permission service for managing admin permissions.
        /// </summary>
        public PermissionService PermissionService { get; }

        /// <summary>
        /// The admin user accessing the page.
        /// </summary>
        public AdminUser AdminUser { get; }

        /// <summary>
        /// The list of flagged technician behavior logs.
        /// </summary>
        public List<DomainTechnicianBehaviorLog> BehaviorLogs { get; set; } = new();

        /// <summary>
        /// The compliance statistics view model.
        /// </summary>
        public ComplianceStatsViewModel ComplianceStats { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TechnicianStatusReportModel"/> class.
        /// </summary>
        /// <param name="auditService">The technician audit service.</param>
        /// <param name="db">The application database context.</param>
        /// <param name="permissionService">The permission service.</param>
        public TechnicianStatusReportModel(Services.Admin.TechnicianAuditService auditService, ApplicationDbContext db, PermissionService permissionService)
        {
            _auditService = auditService;
            _db = db;
            PermissionService = permissionService;
            AdminUser = HttpContext?.Items["AdminUser"] as AdminUser ?? new AdminUser { EnabledModules = new List<string>() };
        }

        /// <summary>
        /// Handles GET requests to the Technician Status Report page.
        /// </summary>
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

        /// <summary>
        /// Represents the compliance statistics view model.
        /// </summary>
        public class ComplianceStatsViewModel
        {
            /// <summary>
            /// The global compliance percentage across all technicians.
            /// </summary>
            public double GlobalCompliancePercent { get; set; }

            /// <summary>
            /// The compliance statistics per technician.
            /// </summary>
            public List<TechnicianComplianceStat> PerTechnician { get; set; } = new();
        }

        /// <summary>
        /// Represents the compliance statistics for a single technician.
        /// </summary>
        public class TechnicianComplianceStat
        {
            /// <summary>
            /// The name of the technician.
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// The compliance percentage of the technician.
            /// </summary>
            public double CompliancePercent { get; set; }

            /// <summary>
            /// The count of flagged logs for the technician.
            /// </summary>
            public int FlaggedCount { get; set; }

            /// <summary>
            /// The count of time violations for the technician.
            /// </summary>
            public int TimeViolations { get; set; }

            /// <summary>
            /// The count of location violations for the technician.
            /// </summary>
            public int LocationViolations { get; set; }

            /// <summary>
            /// The count of metadata violations for the technician.
            /// </summary>
            public int MetadataViolations { get; set; }
        }
    }
}
