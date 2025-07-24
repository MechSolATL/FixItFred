using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace MVP_Core.Pages.Admin.BlazorAdmin.Components
{
    public class DispatcherMapModalModel : PageModel
    {
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string TechName { get; set; } = string.Empty;
        public string TruckIconUrl { get; set; } = "/images/truck-icon.png";
        public string? DestinationAddress { get; set; }
        public bool ShowMap { get; set; }
        public bool FollowTech { get; set; } = true;
        public bool IsSimulated { get; set; }
        public bool IsDevelopment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        // Add any additional logic for modal state as needed
        public void OnGet() { }
    }
}
