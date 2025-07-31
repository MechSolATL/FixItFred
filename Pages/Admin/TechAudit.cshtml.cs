using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;

namespace Pages.Admin
{
    /// <summary>
    /// Represents the Technician Audit page model for viewing and filtering technician behavior logs.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class TechAuditModel : PageModel
    {
        private readonly Services.Admin.TechnicianAuditService _auditService;
        internal readonly ApplicationDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="TechAuditModel"/> class.
        /// </summary>
        /// <param name="db">The application database context.</param>
        /// <param name="auditService">The technician audit service.</param>
        public TechAuditModel(ApplicationDbContext db, Services.Admin.TechnicianAuditService auditService)
        {
            _db = db;
            _auditService = auditService;
        }

        /// <summary>
        /// The ID of the technician to filter logs by.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int? TechnicianId { get; set; }

        /// <summary>
        /// The date to filter logs by.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public DateTime? Date { get; set; }

        /// <summary>
        /// The action type to filter logs by.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public TechnicianAuditActionType? ActionType { get; set; }

        /// <summary>
        /// The list of technician behavior logs matching the filter criteria.
        /// </summary>
        public List<Models.TechnicianBehaviorLog> Logs { get; set; } = new();

        /// <summary>
        /// The list of technicians available for filtering.
        /// </summary>
        public List<Technician> Technicians { get; set; } = new();

        /// <summary>
        /// Handles GET requests to the Technician Audit page.
        /// </summary>
        public async Task OnGetAsync()
        {
            Technicians = _db.Technicians.OrderBy(t => t.FullName).ToList();
            if (TechnicianId.HasValue && Date.HasValue && ActionType.HasValue)
            {
                Logs = await _auditService.GetLogsByTechAndDateAsync(TechnicianId.Value, Date.Value);
                // No ActionType filter for stub, keep as is for now
            }
            else if (TechnicianId.HasValue && Date.HasValue)
            {
                Logs = await _auditService.GetLogsByTechAndDateAsync(TechnicianId.Value, Date.Value);
            }
            else if (TechnicianId.HasValue)
            {
                Logs = await _auditService.GetLogsByRangeAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1), TechnicianId);
            }
            else if (Date.HasValue)
            {
                Logs = await _auditService.GetLogsByRangeAsync(Date.Value.Date, Date.Value.Date.AddDays(1));
            }
            else if (ActionType.HasValue)
            {
                Logs = await _auditService.GetLogsByActionTypeAsync(ActionType.Value.ToString());
            }
            else
            {
                Logs = await _auditService.GetLogsByRangeAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1));
            }
        }

        /// <summary>
        /// Gets the full name of a technician by their ID.
        /// </summary>
        /// <param name="technicianId">The ID of the technician.</param>
        /// <returns>The full name of the technician, or a placeholder if not found.</returns>
        public string GetTechnicianName(int technicianId)
        {
            var tech = Technicians.FirstOrDefault(t => t.Id == technicianId);
            return tech?.FullName ?? $"Tech #{technicianId}";
        }
    }
}
