using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
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
