// FixItFred Patch Log — Sprint 29B-Expand: Admin Manual ETA Override
// [2025-07-25T00:00:00Z] — Added secure admin ETA override logic for multi-zone ETA broadcast.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using MVP_Core.Hubs;
using System.Threading.Tasks;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class OverrideETAModel : PageModel
    {
        private readonly IHubContext<ETAHub> _hubContext;
        public OverrideETAModel(IHubContext<ETAHub> hubContext)
        {
            _hubContext = hubContext;
        }
        [BindProperty]
        public int ZoneId { get; set; }
        [BindProperty]
        public string EtaMessage { get; set; } = string.Empty;
        public string? StatusMessage { get; set; }
        public void OnGet() { }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "Invalid input.";
                return Page();
            }
            await _hubContext.Clients.All.SendAsync("ReceiveETA", ZoneId, EtaMessage);
            StatusMessage = $"ETA sent to zone {ZoneId}.";
            return Page();
        }
    }
}
