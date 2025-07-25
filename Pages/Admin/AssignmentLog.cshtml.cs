// FixItFred Patch Log: Added missing PageModel for AssignmentLog to resolve CS0234 errors.
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Models.Admin;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class AssignmentLogModel : PageModel
    {
        public List<AssignmentLogEntry> AssignmentEntries { get; set; } = new();
        public void OnGet()
        {
            // Populate AssignmentEntries from service or database as needed
        }
    }
}
