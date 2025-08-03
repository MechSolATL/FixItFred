using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Services.FixItFred;
using MVP_Core.Models.FixItFred;

namespace MVP_Core.Pages.Admin;

public class CommandCenterModel : PageModel
{
    private readonly IFixItFredService _fixItFredService;
    private readonly ILogger<CommandCenterModel> _logger;

    [BindProperty]
    public string TriggerSource { get; set; } = "ManualForce";

    [BindProperty]
    public string SweepTag { get; set; } = "vOmegaFinal_PR22";

    [BindProperty]
    public string? RelatedPr { get; set; } = "PR22";

    [BindProperty]
    public string? CurrentSweepId { get; set; }

    public OmegaSweepStatus? CurrentStatus { get; set; }

    public CommandCenterModel(IFixItFredService fixItFredService, ILogger<CommandCenterModel> logger)
    {
        _fixItFredService = fixItFredService;
        _logger = logger;
    }

    public async Task OnGetAsync(string? sweepId = null)
    {
        if (!string.IsNullOrEmpty(sweepId))
        {
            CurrentSweepId = sweepId;
            CurrentStatus = await _fixItFredService.GetSweepStatusAsync(sweepId);
        }
    }

    public async Task<IActionResult> OnPostStartSweepAsync()
    {
        if (string.IsNullOrWhiteSpace(SweepTag))
        {
            ModelState.AddModelError(nameof(SweepTag), "Sweep tag is required");
            return Page();
        }

        try
        {
            var status = await _fixItFredService.StartOmegaSweepAsync(TriggerSource, SweepTag, RelatedPr);
            CurrentSweepId = status.SweepId;
            CurrentStatus = status;

            _logger.LogInformation("OmegaSweep initiated from CommandCenter: {SweepId}", status.SweepId);
            
            return RedirectToPage("CommandCenter", new { sweepId = status.SweepId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting OmegaSweep from CommandCenter");
            ModelState.AddModelError("", "Failed to start OmegaSweep. Please check logs for details.");
            return Page();
        }
    }

    public async Task<IActionResult> OnPostApproveLockTagAsync()
    {
        if (string.IsNullOrEmpty(CurrentSweepId))
        {
            ModelState.AddModelError("", "No active sweep to approve");
            return Page();
        }

        try
        {
            var success = await _fixItFredService.ApproveLockTagAsync(CurrentSweepId, SweepTag);
            if (success)
            {
                CurrentStatus = await _fixItFredService.GetSweepStatusAsync(CurrentSweepId);
                _logger.LogInformation("Tag {SweepTag} approved and unlocked via CommandCenter", SweepTag);
            }
            else
            {
                ModelState.AddModelError("", "Failed to approve tag. Sweep may not be completed successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving tag lock from CommandCenter");
            ModelState.AddModelError("", "Failed to approve tag lock. Please check logs for details.");
        }

        return Page();
    }

    public async Task<IActionResult> OnGetStatusApiAsync(string sweepId)
    {
        var status = await _fixItFredService.GetSweepStatusAsync(sweepId);
        return new JsonResult(status);
    }
}