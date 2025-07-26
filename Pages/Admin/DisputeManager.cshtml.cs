using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MVP_Core.Pages.Admin
{
    public class DisputeManagerModel : PageModel
    {
        private readonly DisputeService _disputeService;
        public List<DisputeRecord> FilteredDisputes { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Reason { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? EscalationLevel { get; set; }
        public DisputeManagerModel(DisputeService disputeService)
        {
            _disputeService = disputeService;
        }
        public void OnGet()
        {
            FilteredDisputes = _disputeService.FilterDisputes(Status, Reason, EscalationLevel);
        }
        public IActionResult OnPost()
        {
            int.TryParse(Request.Form["DisputeId"], out var disputeId);
            var status = Request.Form["Status"];
            var resolutionNotes = Request.Form["ResolutionNotes"];
            var reviewedBy = Request.Form["ReviewedBy"];
            int.TryParse(Request.Form["EscalationLevel"], out var escalationLevel);
            _disputeService.UpdateDisputeStatus(disputeId, status, resolutionNotes, reviewedBy, escalationLevel);
            OnGet();
            return Page();
        }
    }
}
