using Microsoft.AspNetCore.Mvc.RazorPages;
using MVP_Core.Data.Models;
using MVP_Core.Services;
using System;
using System.Collections.Generic;

namespace MVP_Core.Pages.Admin
{
    public class QuickTestModel : PageModel
    {
        public string TestResult { get; set; } = string.Empty;
        public void OnGet()
        {
            // Arrange: create fake requests
            var requests = new List<ServiceRequest>
            {
                new ServiceRequest { AssignedTechnicianId = 1, CreatedAt = new DateTime(2024, 1, 10), Status = "Complete", ScheduledAt = new DateTime(2024, 1, 10, 8, 0, 0), ClosedAt = new DateTime(2024, 1, 10, 8, 0, 0) },
                new ServiceRequest { AssignedTechnicianId = 1, CreatedAt = new DateTime(2024, 1, 15), Status = "Complete", ScheduledAt = new DateTime(2024, 1, 15, 8, 0, 0), ClosedAt = new DateTime(2024, 1, 15, 8, 10, 0) },
                new ServiceRequest { AssignedTechnicianId = 1, CreatedAt = new DateTime(2024, 2, 5), Status = "Assigned", ScheduledAt = new DateTime(2024, 2, 5, 8, 0, 0), ClosedAt = new DateTime(2024, 2, 5, 8, 30, 0), NeedsFollowUp = true },
                new ServiceRequest { AssignedTechnicianId = 1, CreatedAt = new DateTime(2024, 2, 20), Status = "Complete", ScheduledAt = new DateTime(2024, 2, 20, 8, 0, 0), ClosedAt = new DateTime(2024, 2, 20, 7, 55, 0) },
            };
            // Act
            var (closeRates, callbacks, etaSuccess, avgDelay) = TechnicianProfileService.CalculateKpis(requests);
            // Assert (manual)
            bool pass = closeRates.Count == 2 && callbacks.Count == 1 && etaSuccess > 0 && avgDelay > 0;
            TestResult = $"CloseRates: {closeRates.Count}, Callbacks: {callbacks.Count}, ETA: {etaSuccess:P2}, AvgDelay: {avgDelay:F1} min, PASS: {pass}";
        }
    }
}
