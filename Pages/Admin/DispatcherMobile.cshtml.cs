// Sprint 49.0 Patch Log: Mobile Dispatcher View PageModel
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;
using MVP_Core.Models.Admin;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class DispatcherMobileModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        public DispatcherMobileModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }
        public List<RequestSummaryDto> Requests { get; set; } = new();
        public void OnGet()
        {
            Requests = _dispatcherService.GetFilteredRequests(new DispatcherFilterModel());
        }
        public string GetStatusClass(string status)
        {
            return status switch
            {
                "Open" => "warning",
                "Assigned" => "primary",
                "En Route" => "info",
                "Complete" => "success",
                _ => "secondary"
            };
        }
    }
}
// End Sprint 49.0 Patch Log
