using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MVP_Core.Models.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Admin;

namespace Pages.Admin
{
    public class SlaTrendsModel : PageModel
    {
        private readonly DispatcherService _dispatcherService;
        public SlaTrendsModel(DispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }
        public void OnGet() { }

        public async Task<IActionResult> OnGetSlaTrendsAsync()
        {
            var data = await _dispatcherService.GetSlaTrendsAsync();
            return new JsonResult(data);
        }
    }
}
