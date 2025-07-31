using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Admin;
using Data;
using Services.Admin;

namespace Pages.Admin
{
    [Authorize(Roles = "Dispatcher,Supervisor")]
    public class CalendarModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        private readonly ApplicationDbContext _db;

        public CalendarModel(DispatcherService dispatcherService, ApplicationDbContext db)
        {
            _dispatcherService = dispatcherService;
            _db = db;
        }

        public List<RequestSummaryDto> Requests { get; set; } = new();
        public List<TechnicianStatusDto> Technicians { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Load all jobs and technicians for the calendar view
            Requests = _dispatcherService.GetFilteredRequests(new DispatcherFilterModel());
            Technicians = _dispatcherService.GetAllTechnicianStatuses();
            await Task.CompletedTask;
        }
    }
}
