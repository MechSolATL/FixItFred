using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Data;
using MVP_Core.Models.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    /// <summary>
    /// Represents the Override Center page model for managing technician assignments and job zones.
    /// </summary>
    [Authorize(Roles = "Admin,Dispatcher")]
    public class OverrideCenterModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideCenterModel"/> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service for managing technician assignments.</param>
        /// <param name="db">The application database context.</param>
        public OverrideCenterModel(DispatcherService dispatcherService, ApplicationDbContext db)
        {
            _dispatcherService = dispatcherService;
            _db = db;
        }

        /// <summary>
        /// The list of service zones available for rerouting jobs.
        /// </summary>
        public List<string> ServiceZones { get; set; } = new List<string> { "North", "South", "East", "West" };

        /// <summary>
        /// The list of all technicians and their statuses.
        /// </summary>
        public List<TechnicianStatusDto> AllTechnicians { get; set; } = new();

        /// <summary>
        /// The ID of the job to be modified.
        /// </summary>
        [BindProperty]
        public int JobId { get; set; }

        /// <summary>
        /// The new zone to assign to the job.
        /// </summary>
        [BindProperty]
        public string? NewZone { get; set; }

        /// <summary>
        /// The ID of the technician to be assigned to the job.
        /// </summary>
        [BindProperty]
        public int TechnicianId { get; set; }

        /// <summary>
        /// The ID of the message to be rewritten.
        /// </summary>
        [BindProperty]
        public int MessageId { get; set; }

        /// <summary>
        /// The new content of the message.
        /// </summary>
        [BindProperty]
        public string? NewMessage { get; set; }

        /// <summary>
        /// The status message for zone rerouting operations.
        /// </summary>
        public string? ZoneRerouteMessage { get; set; }

        /// <summary>
        /// The status message for force assignment operations.
        /// </summary>
        public string? ForceAssignMessage { get; set; }

        /// <summary>
        /// The status message for message rewriting operations.
        /// </summary>
        public string? RewriteMessageStatus { get; set; }

        /// <summary>
        /// Handles GET requests to the Override Center page.
        /// </summary>
        public void OnGet()
        {
            AllTechnicians = _dispatcherService.GetAllTechnicianStatuses();
        }

        /// <summary>
        /// Handles POST requests for rerouting job zones.
        /// </summary>
        /// <returns>The page result.</returns>
        public IActionResult OnPostZoneReroute()
        {
            AllTechnicians = _dispatcherService.GetAllTechnicianStatuses();
            if (JobId > 0 && !string.IsNullOrEmpty(NewZone))
            {
                bool success = _dispatcherService.OverrideJobZone(JobId, NewZone);
                ZoneRerouteMessage = success ? "Job zone updated." : "Failed to update job zone.";
            }
            else
            {
                ZoneRerouteMessage = "Invalid input.";
            }
            return Page();
        }

        /// <summary>
        /// Handles POST requests for force assigning technicians to jobs.
        /// </summary>
        /// <returns>The page result.</returns>
        public async Task<IActionResult> OnPostForceAssign()
        {
            AllTechnicians = _dispatcherService.GetAllTechnicianStatuses();
            if (JobId > 0 && TechnicianId > 0)
            {
                bool success = await _dispatcherService.ReassignTechnician(JobId, TechnicianId);
                ForceAssignMessage = success ? "Technician forcibly assigned." : "Failed to assign technician.";
            }
            else
            {
                ForceAssignMessage = "Invalid input.";
            }
            return Page();
        }

        /// <summary>
        /// Handles POST requests for rewriting job messages.
        /// </summary>
        /// <returns>The page result.</returns>
        public IActionResult OnPostRewriteMessage()
        {
            AllTechnicians = _dispatcherService.GetAllTechnicianStatuses();
            if (MessageId > 0 && !string.IsNullOrEmpty(NewMessage))
            {
                var msg = _db.JobMessages.FirstOrDefault(m => m.Id == MessageId);
                if (msg != null)
                {
                    msg.Message = NewMessage;
                    _db.SaveChanges();
                    RewriteMessageStatus = "Message updated.";
                }
                else
                {
                    RewriteMessageStatus = "Message not found.";
                }
            }
            else
            {
                RewriteMessageStatus = "Invalid input.";
            }
            return Page();
        }
    }
}
