using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
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
