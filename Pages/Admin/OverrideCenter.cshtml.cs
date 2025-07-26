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
    [Authorize(Roles = "Admin,Dispatcher")]
    public class OverrideCenterModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        private readonly ApplicationDbContext _db;
        public OverrideCenterModel(DispatcherService dispatcherService, ApplicationDbContext db)
        {
            _dispatcherService = dispatcherService;
            _db = db;
        }
        public List<string> ServiceZones { get; set; } = new List<string> { "North", "South", "East", "West" };
        public List<TechnicianStatusDto> AllTechnicians { get; set; } = new();
        [BindProperty] public int JobId { get; set; }
        [BindProperty] public string? NewZone { get; set; }
        [BindProperty] public int TechnicianId { get; set; }
        [BindProperty] public int MessageId { get; set; }
        [BindProperty] public string? NewMessage { get; set; }
        public string? ZoneRerouteMessage { get; set; }
        public string? ForceAssignMessage { get; set; }
        public string? RewriteMessageStatus { get; set; }
        public void OnGet()
        {
            AllTechnicians = _dispatcherService.GetAllTechnicianStatuses();
        }
        public IActionResult OnPostZoneReroute()
        {
            AllTechnicians = _dispatcherService.GetAllTechnicianStatuses();
            if (JobId > 0 && !string.IsNullOrEmpty(NewZone))
            {
                // For demo, treat zone as string, not int
                bool success = _dispatcherService.OverrideJobZone(JobId, NewZone);
                ZoneRerouteMessage = success ? "Job zone updated." : "Failed to update job zone.";
            }
            else
            {
                ZoneRerouteMessage = "Invalid input.";
            }
            return Page();
        }
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
        public IActionResult OnPostRewriteMessage()
        {
            AllTechnicians = _dispatcherService.GetAllTechnicianStatuses();
            if (MessageId > 0 && !string.IsNullOrEmpty(NewMessage))
            {
                // Update JobMessageEntry in DB
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
