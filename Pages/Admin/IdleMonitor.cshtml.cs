using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class IdleMonitorModel : PageModel
    {
        private readonly IdleSessionMonitorService _service;
        public IdleMonitorModel(IdleSessionMonitorService service)
        {
            _service = service;
        }
        [BindProperty(SupportsGet = true)]
        public int TechnicianId { get; set; }
        public List<IdleSessionMonitorLog> Logs { get; set; } = new();
        public void OnGet()
        {
            Logs = TechnicianId > 0 ? _service.GetIdleSessions(TechnicianId) : _service.GetAllLogs();
        }
    }
}
