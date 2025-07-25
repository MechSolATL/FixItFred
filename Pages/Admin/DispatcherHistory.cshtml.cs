// FixItFred Patch Log: Added missing PageModel for DispatcherHistory to resolve CS0234 errors.
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models.Admin;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class DispatcherHistoryModel : PageModel
    {
        public List<RequestSummaryDto> HistoryEntries { get; set; } = new();
        public void OnGet()
        {
            // Populate HistoryEntries from service or database as needed
        }
    }
}
