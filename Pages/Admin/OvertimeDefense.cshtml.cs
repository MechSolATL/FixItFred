using MVP_Core.Services.Admin;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class OvertimeDefenseModel : PageModel
    {
        private readonly OvertimeDefenseService _service;
        public OvertimeDefenseModel(OvertimeDefenseService service)
        {
            _service = service;
        }
        [BindProperty(SupportsGet = true)]
        public int TechnicianId { get; set; }
        public List<OvertimeLockoutLog> Logs { get; set; } = new();
        public void OnGet()
        {
            Logs = TechnicianId > 0 ? _service.DetectOverworkedSessions(TechnicianId) : _service.GetAllLogs();
        }
    }
}
