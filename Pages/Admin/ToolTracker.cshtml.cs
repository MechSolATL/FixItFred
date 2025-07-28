using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Data.DTO.Tools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin,Dispatcher")] // Sprint 91.7 Part 6.3
    public class ToolTrackerModel : PageModel
    {
        private readonly ToolTrackingService _toolTrackingService;
        private readonly PermissionService _permissionService;

        public ToolTrackerModel(ToolTrackingService toolTrackingService, PermissionService permissionService)
        {
            _toolTrackingService = toolTrackingService;
            _permissionService = permissionService;
        }

        public List<ToolDto> Tools { get; set; } = new();
        public List<SelectListItem> TechnicianOptions { get; set; } = new();
        public List<SelectListItem> StatusOptions { get; set; } = new();
        public List<SelectListItem> AvailableTechs { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? FilterStatus { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? FilterTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        public string? StatusMessage { get; set; }
        public string? ErrorMessage { get; set; }

        // Sprint 91.7 Part 6.3: Load tool inventory and filter options
        public async Task OnGetAsync()
        {
            var allTools = await _toolTrackingService.GetAllToolsAsync();
            Tools = allTools;
            if (!string.IsNullOrEmpty(FilterStatus) && FilterStatus != "All")
                Tools = Tools.Where(t => t.Status == FilterStatus).ToList();
            if (!string.IsNullOrEmpty(FilterTechnicianId) && Guid.TryParse(FilterTechnicianId, out var techId))
                Tools = Tools.Where(t => t.AssignedTechnicianId == techId).ToList();
            if (!string.IsNullOrEmpty(SearchTerm))
                Tools = Tools.Where(t => t.Name.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase)).ToList();
            TechnicianOptions = allTools.Where(t => t.AssignedTechnicianId != null && !string.IsNullOrEmpty(t.AssignedTechnicianName))
                .GroupBy(t => new { t.AssignedTechnicianId, t.AssignedTechnicianName })
                .Select(g => new SelectListItem { Value = g.Key.AssignedTechnicianId.ToString()!, Text = g.Key.AssignedTechnicianName! })
                .ToList();
            TechnicianOptions.Insert(0, new SelectListItem { Value = "", Text = "All" });
            StatusOptions = new List<SelectListItem> {
                new SelectListItem { Value = "All", Text = "All" },
                new SelectListItem { Value = "Available", Text = "Available" },
                new SelectListItem { Value = "InUse", Text = "Assigned" },
                new SelectListItem { Value = "Inactive", Text = "Inactive" }
            };
            // For transfer modal: show all available techs
            var techs = await _toolTrackingService.GetAvailableTechsAsync();
            AvailableTechs = techs.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.FullName }).ToList();
        }

        // Sprint 91.7 Part 6.3: Handle tool transfer post
        public async Task<IActionResult> OnPostTransferToolAsync(Guid ToolId, Guid FromTechnicianId, Guid ToTechnicianId, string? TransferNotes)
        {
            var dto = new ToolTransferRequestDto
            {
                ToolId = ToolId,
                FromTechnicianId = FromTechnicianId,
                ToTechnicianId = ToTechnicianId,
                TransferNotes = TransferNotes
            };
            var result = await _toolTrackingService.TransferToolAsync(dto);
            if (result)
            {
                StatusMessage = "Tool transfer request submitted.";
            }
            else
            {
                ErrorMessage = "Failed to submit tool transfer. Please check tool and technician selection.";
            }
            await OnGetAsync();
            return Page();
        }
    }
}
