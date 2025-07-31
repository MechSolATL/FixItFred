using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.LiveTracking;
using MVP_Core.Services.LiveTracking.Models;

public class LiveMapModel : PageModel
{
    private readonly ITechTrackerService _tracker;

    public TechnicianLocation? Technician { get; set; }

    public LiveMapModel(ITechTrackerService tracker)
    {
        _tracker = tracker;
    }

    public void OnGet()
    {
        // For demo: static technician ID
        Technician = _tracker.GetTechnicianLocation(Guid.Parse("de305d54-75b4-431b-adb2-eb6b9e546014"));
    }
}