// Copyright (c) MechSolATL. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVP_Core.Services.Pages;

/// <summary>
/// Page model for Command Center dashboard
/// </summary>
public class CommandCenterModel : PageModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandCenterModel"/> class.
    /// </summary>
    public CommandCenterModel()
    {
    }

    /// <summary>
    /// Handles GET request and loads SEO metadata
    /// </summary>
    /// <returns>Page result</returns>
    public async Task OnGetAsync()
    {
        // Load SEO metadata for Command Center page
        ViewData["Title"] = "Command Center - Sprint122_CertumDNSBypass";
        ViewData["MetaDescription"] = "FixItFred Command Center dashboard for monitoring Sprint122_CertumDNSBypass field-first development progress and system status.";
        ViewData["Keywords"] = "FixItFred, Command Center, Sprint122, CertumDNSBypass, Field First, MVP Core, Dashboard";
        ViewData["Robots"] = "index,follow";

        await Task.CompletedTask;
    }
}