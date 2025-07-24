using Microsoft.AspNetCore.Mvc;
using MVP_Core.Services.Admin;
using MVP_Core.Models.Mobile;

public class NextJobModel : PageModel
{
    private readonly DispatcherService _dispatcherService;
    public NextJobModel(DispatcherService dispatcherService)
    {
        _dispatcherService = dispatcherService;
    }
    [BindProperty]
    public int TechId { get; set; }
    public NextJobDto? Job { get; set; }
    public IActionResult OnGet(int techId)
    {
        TechId = techId;
        Job = _dispatcherService.GetNextJobForTechnician(techId);
        return Page();
    }
    public IActionResult OnPostPingDispatcher()
    {
        _dispatcherService.UpdateTechnicianPing(TechId);
        TempData["PingStatus"] = "? Dispatcher Notified";
        Job = _dispatcherService.GetNextJobForTechnician(TechId);
        return Page();
    }
}
