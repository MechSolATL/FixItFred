using MVP_Core.Data.Models;
using MVP_Core.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Pages.Technician
{
    // FixItFred: Sprint 30E.2 - GPS Update Logic
    [Authorize(Roles = "Technician")]
    public class UpdateLocationModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int TechnicianId { get; set; }

        public void OnGet()
        {
            // Set TechnicianId from session/claims if available
            if (User.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (idClaim != null && int.TryParse(idClaim.Value, out int tid))
                    TechnicianId = tid;
            }
        }
    }
}
