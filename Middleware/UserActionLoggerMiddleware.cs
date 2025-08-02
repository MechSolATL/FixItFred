using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Models;
using Data.Models;
using System.Security.Claims;
using Services.Analytics;

namespace Middleware
{
    // Sprint 86.5 — Middleware to log every user action for flow compliance
    public class UserActionLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ActionLogService _actionLogService;

        public UserActionLoggerMiddleware(RequestDelegate next, ActionLogService actionLogService)
        {
            _next = next;
            _actionLogService = actionLogService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;
            int adminUserId = 0;
            if (user.Identity?.IsAuthenticated == true)
            {
                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(idClaim, out adminUserId);
            }
            var log = new UserActionLog
            {
                AdminUserId = adminUserId,
                ActionType = "PageRequest",
                PageName = context.Request.Path.Value ?? string.Empty,
                Route = context.Request.Path.Value ?? string.Empty,
                Timestamp = DateTime.UtcNow,
                SessionId = context.Session?.Id ?? string.Empty,
                IsError = false,
                Detail = context.Request.Method
            };
            await _actionLogService.LogActionAsync(log);
            await _next(context);
        }
    }
}
