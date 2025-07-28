using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.Admin;

namespace MVP_Core.Pages.Admin
{
    [Authorize(Roles = "Admin,Dispatcher")]
    public class ManagerToolsModel : PageModel
    {
        private readonly ManagerInterventionService _managerService;
        public ManagerToolsModel(ManagerInterventionService managerService)
        {
            _managerService = managerService;
        }
        public void OnGet()
        {
            // Initialization logic will go here
        }
    }
}
