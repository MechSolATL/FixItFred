// Sprint 84.0 — Feature Access Block Enforcement
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVP_Core.Data.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace Middleware
{
    public class RequireProsCertificationAttribute : Attribute, IPageFilter
    {
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var db = context.HttpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var user = context.HttpContext.User;
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || db == null)
            {
                context.Result = new RedirectToPageResult("/AccessDenied", new { message = "Unable to verify certification status." });
                return;
            }
            var onboarding = db.UserOnboardingStatuses.FirstOrDefault(u => u.UserId.ToString() == userId);
            if (onboarding == null || !onboarding.IsProsCertified)
            {
                context.Result = new RedirectToPageResult("/Onboarding/Start", new { message = "?? Onboarding Incomplete — Access Limited. PROS Certification required." });
            }
        }
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
        public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
    }
}
