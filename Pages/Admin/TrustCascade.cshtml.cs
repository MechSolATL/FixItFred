using MVP_Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVP_Core.Services.Admin;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVP_Core.Pages.Admin
{
    public class TrustCascadeModel : PageModel
    {
        private readonly TrustCascadeEngine _engine;
        public List<TrustCascadeLog> Cascades { get; set; } = new();
        public TrustCascadeModel(TrustCascadeEngine engine) { _engine = engine; }
        public async Task OnGetAsync()
        {
            Cascades = await _engine.GetTrustCascadesAsync();
        }
    }
}
