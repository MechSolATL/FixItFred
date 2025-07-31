using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using Data.Models;
using Services;

namespace Pages.Admin
{
    public class DisputeManagerModel : PageModel
    {
        private readonly DisputeService _disputeService; // Sprint 78.9: Null safety and initialization
        public List<DisputeRecord> FilteredDisputes { get; set; } = new(); // Sprint 78.9: Null safety and initialization
        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; } // Sprint 78.9: Null safety and initialization
        [BindProperty(SupportsGet = true)]
        public string? Reason { get; set; } // Sprint 78.9: Null safety and initialization
        [BindProperty(SupportsGet = true)]
        public int? EscalationLevel { get; set; } // Sprint 78.9: Null safety and initialization
        public DisputeManagerModel(DisputeService disputeService)
        {
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService)); // Sprint 78.9: Null safety and initialization
        }
        public void OnGet()
        {
            FilteredDisputes = _disputeService.FilterDisputes(Status, Reason, EscalationLevel); // Sprint 78.9: Null safety and initialization
        }
        public IActionResult OnPost()
        {
            var userId = User?.Identity?.Name ?? "anonymous"; // Sprint 78.9: Null safety and initialization
            int.TryParse(Request?.Form["DisputeId"].ToString(), out var disputeId); // Sprint 78.9: Null safety and initialization
            var status = Request?.Form["Status"].ToString() ?? string.Empty; // Sprint 78.9: Null safety and initialization
            var resolutionNotes = Request?.Form["ResolutionNotes"].ToString() ?? string.Empty; // Sprint 78.9: Null safety and initialization
            var reviewedBy = Request?.Form["ReviewedBy"].ToString() ?? string.Empty; // Sprint 78.9: Null safety and initialization
            int.TryParse(Request?.Form["EscalationLevel"].ToString(), out var escalationLevel); // Sprint 78.9: Null safety and initialization
            _disputeService.UpdateDisputeStatus(disputeId, status, resolutionNotes, reviewedBy, escalationLevel); // Sprint 78.9: Null safety and initialization
            OnGet(); // Sprint 78.9: Null safety and initialization
            return Page(); // Sprint 78.9: Null safety and initialization
        }
    }
}
