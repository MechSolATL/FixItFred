using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVP_Core.Pages.Admin
{
    public class GhostAuditLogModel : PageModel
    {
        private readonly GhostDelayAuditor _auditor;
        public List<DepartmentDelayLog> SampledDelays { get; set; } = new();
        public GhostAuditLogModel(GhostDelayAuditor auditor) { _auditor = auditor; }
        public async Task OnGetAsync()
        {
            SampledDelays = await _auditor.SampleCompletedDelaysAsync();
        }
        public bool IsShadowDelay(DepartmentDelayLog log) => _auditor.IsShadowDelay(log);
    }
}
