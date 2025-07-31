using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class GpsDriftLogModel : PageModel
    {
        private readonly GpsDriftDetectorService _driftService;
        public GpsDriftLogModel(GpsDriftDetectorService driftService)
        {
            _driftService = driftService;
        }

        public List<GpsDriftEventLog> DriftLogs { get; set; } = new();
        [BindProperty(SupportsGet = true)] public int? FilterTechnicianId { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? FilterFromDate { get; set; }
        [BindProperty(SupportsGet = true)] public double? FilterMinDistance { get; set; }

        public async Task OnGetAsync()
        {
            DriftLogs = await _driftService.GetRecentDriftEvents(FilterFromDate, FilterTechnicianId, FilterMinDistance);
        }
    }
}