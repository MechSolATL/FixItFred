using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
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
