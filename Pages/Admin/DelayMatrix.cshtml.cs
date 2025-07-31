using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using Services.Admin;

namespace Pages.Admin
{
    public class DelayMatrixModel : PageModel
    {
        private readonly DelayMatrixService _service;
        public List<DepartmentDelayLog> Delays { get; set; } = new();
        [BindProperty(SupportsGet = true)] public int MinMinutes { get; set; } = 0;
        public DelayMatrixModel(DelayMatrixService service) { _service = service; }
        public async Task OnGetAsync()
        {
            Delays = await _service.GetDelaysAsync(MinMinutes);
        }
    }
}
