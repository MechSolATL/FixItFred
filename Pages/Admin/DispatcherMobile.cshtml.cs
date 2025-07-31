// Sprint 49.0 Patch Log: Mobile Dispatcher View PageModel
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Models.Admin;
using Services.Admin;

namespace Pages.Admin
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
