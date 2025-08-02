using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Admin;

namespace Pages.Admin
{
    public class InactivityMapModel : PageModel
    {
        private readonly InactivityHeatmapAgent _agent;
        public Dictionary<string, int> InactivityMap { get; set; } = new();
        public InactivityMapModel(InactivityHeatmapAgent agent) { _agent = agent; }
        public async Task OnGetAsync()
        {
            InactivityMap = await _agent.GetDepartmentInactivityAsync();
        }
    }
}
