using MVP_Core.Services.Admin;
using MVP_Core.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class GeoBreakAuditModel : PageModel
    {
        private readonly GeoBreakValidatorService _service;
        public GeoBreakAuditModel(GeoBreakValidatorService service)
        {
            _service = service;
        }
        [BindProperty(SupportsGet = true)]
        public int TechnicianId { get; set; }
        public List<GeoBreakValidationLog> Logs { get; set; } = new();
        public void OnGet()
        {
            Logs = TechnicianId > 0 ? _service.GetAllLogs().FindAll(l => l.TechnicianId == TechnicianId) : _service.GetAllLogs();
        }
    }
}
