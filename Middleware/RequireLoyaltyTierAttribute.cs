// Sprint 84.2 — Tier Enforcement
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Data.Models;
using Services.Admin;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Middleware
{
    public class RequireLoyaltyTierAttribute : Attribute, IAsyncPageFilter
    {
        public string MinTier { get; }
        public RequireLoyaltyTierAttribute(string minTier)
        {
            MinTier = minTier;
        }
        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var db = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var loyaltyService = context.HttpContext.RequestServices.GetService(typeof(TechnicianLoyaltyService)) as TechnicianLoyaltyService;
            var user = context.HttpContext.User;
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || db == null || loyaltyService == null)
            {
                context.Result = new RedirectToPageResult("/AccessDenied", new { message = "Unable to verify loyalty tier." });
                return;
            }
            var tech = db.Technicians.FirstOrDefault(t => t.Email == user.Identity.Name);
            if (tech == null)
            {
                context.Result = new RedirectToPageResult("/AccessDenied", new { message = "Technician profile not found." });
                return;
            }
            // Manager override flag (e.g., IsManagerOverride)
            var isManagerOverride = user.IsInRole("Manager") || (tech.GetType().GetProperty("IsManagerOverride")?.GetValue(tech) as bool? ?? false);
            if (isManagerOverride)
            {
                await next();
                return;
            }
            var tier = await loyaltyService.GetTierAsync(tech.Id);
            var allowedTiers = new[] { "None", "Bronze", "Silver", "Gold", "Platinum" };
            var minTierIndex = Array.IndexOf(allowedTiers, MinTier);
            var userTierIndex = Array.IndexOf(allowedTiers, tier);
            if (userTierIndex < minTierIndex)
            {
                context.Result = new RedirectToPageResult("/AccessDenied", new { message = $"Access requires {MinTier} tier or higher. Your tier: {tier}." });
                return;
            }
            await next();
        }
        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
