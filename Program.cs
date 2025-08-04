// © 1997–2025 Virtual Concepts LLC, All Rights Reserved.
// Created & designed by Virtual Concepts LLC for Mechanical Solutions Atlanta.
// Platform: Service-Atlanta.com (MVP-Core vOmegaFinal)
// Use is strictly limited to verified users who have completed Service Atlanta's full verification process.
// Unauthorized use without written authorization is enforceable by law.

using Microsoft.EntityFrameworkCore;
using Wangkanai.Detection;
using Data;

var builder = WebApplication.CreateBuilder(args);

// Razor + HttpContext
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// ---------- Strongly-known MVP_Core services (present in repo) ----------
builder.Services.AddScoped<MVP_Core.Services.ISeoService, MVP_Core.Services.SEOService>();
builder.Services.AddScoped<MVP_Core.Services.CertificationService>();
builder.Services.AddScoped<MVP_Core.Services.SkillsTrackerService>();
builder.Services.AddScoped<MVP_Core.Services.IContentService, MVP_Core.Services.ContentService>();
builder.Services.AddScoped<MVP_Core.Services.IFXQuoteShuffleService, MVP_Core.Services.FXQuoteShuffleService>();
builder.Services.AddScoped<MVP_Core.Services.ISparks88VoiceService, MVP_Core.Services.Sparks88VoiceService>();

builder.Services.AddScoped<MVP_Core.Services.ICalendarSyncService, MVP_Core.Services.Integrations.GoogleCalendarSyncService>();
builder.Services.AddScoped<MVP_Core.Services.Integrations.GoogleCalendarSyncService>();
builder.Services.AddScoped<MVP_Core.Services.Integrations.OutlookCalendarSyncService>();
builder.Services.AddScoped<MVP_Core.Services.FieldAssessmentReportService>();
builder.Services.AddScoped<MVP_Core.Services.TechViewPatchOverlayService>();
builder.Services.AddScoped<MVP_Core.Services.Diagnostics.IServiceModuleScanner, MVP_Core.Services.Diagnostics.ServiceModuleScanner>();
builder.Services.AddScoped<MVP_Core.Services.Diagnostics.DiagnosticsRunner>();

// Optional MVP_Core FixItFred app/services (skip if not present locally)
TypeRegistrar.TryAddScoped(builder.Services,
    "MVP_Core.Services.FixItFred.IFixItFredService",
    "MVP_Core.Services.FixItFred.FixItFredService");
TypeRegistrar.TryAddScoped(builder.Services, "MVP_Core.FixItFred.CI.FixItFredApp");

// ---------- Back-compat service names (present in repo) ----------
builder.Services.AddScoped<Services.ISeoService, Services.SEOService>();
builder.Services.AddScoped<Services.CertificationService>();
builder.Services.AddScoped<Services.SkillsTrackerService>();
builder.Services.AddScoped<Services.IContentService, Services.ContentService>();
builder.Services.AddScoped<Services.System.SystemDiagnosticsService>();
builder.Services.AddScoped<Services.TelemetryTraceService>();

// ---------- Optional/variable services (register only if types exist) ----------
TypeRegistrar.TryAddScoped(builder.Services, "Interfaces.IUserContext", "Services.DefaultUserContext");

// Admin / Diagnostics
TypeRegistrar.TryAddScoped(builder.Services, "Services.Admin.ISmartAdminAlertsService", "Services.Admin.SmartAdminAlertsService");
TypeRegistrar.TryAddScoped(builder.Services, "Services.Admin.AutoRepairEngine");

// Notifications
TypeRegistrar.TryAddScoped(builder.Services,
    "MVP_Core.Services.INotificationSchedulerService",
    "MVP_Core.Services.NotificationSchedulerService");
TypeRegistrar.TryAddScoped(builder.Services, "Services.Dispatch.INotificationDispatchEngine", "Services.Dispatch.NotificationDispatchEngine");

// Analytics / Stats
TypeRegistrar.TryAddScoped(builder.Services, "Services.ICustomerTicketAnalyticsService", "Services.CustomerTicketAnalyticsService");
TypeRegistrar.TryAddScoped(builder.Services, "Services.ISkillLeaderboardService", "Services.SkillLeaderboardService");
TypeRegistrar.TryAddScoped(builder.Services, "Services.Stats.ILeaderboardService", "Services.Stats.LeaderboardService");

// Legacy simulator (if still around)
TypeRegistrar.TryAddScoped(builder.Services, "Services.Admin.ValidationSimulatorService");

// SignalR + Device detection
builder.Services.AddSignalR();
builder.Services.AddDetection();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

// Optional hub mapping: only if OmegaSweepHub exists
TypeRegistrar.TryMapHubIfExists(app, "MVP_Core.Hubs.OmegaSweepHub", "/omegaSweepHub");

app.Run();

// Required for WebApplicationFactory test hosting
public partial class Program { }

// --------------------- helpers ---------------------
static class TypeRegistrar
{
    public static void TryAddScoped(IServiceCollection services, string implementationTypeName)
    {
        var impl = ResolveType(implementationTypeName);
        if (impl != null) services.AddScoped(impl);
    }

    public static void TryAddScoped(IServiceCollection services, string serviceTypeName, string implementationTypeName)
    {
        var service = ResolveType(serviceTypeName);
        var impl = ResolveType(implementationTypeName);

        if (service != null && impl != null)
            services.AddScoped(service, impl);
        else if (impl != null)
            services.AddScoped(impl);
        // else: skip silently
    }

    public static void TryMapHubIfExists(WebApplication app, string hubTypeName, string pattern)
    {
        var hubT = ResolveType(hubTypeName);
        if (hubT == null) return;

        // MapHub(Type, string) overload exists in SignalR extensions
        var mapHub = typeof(Microsoft.AspNetCore.Builder.HubEndpointRouteBuilderExtensions)
            .GetMethods()
            .FirstOrDefault(m =>
                m.Name == "MapHub" &&
                m.GetParameters() is var ps &&
                ps.Length >= 2 &&
                ps[0].ParameterType.Name.Contains("IEndpointRouteBuilder") &&
                ps[1].ParameterType == typeof(Type));

        if (mapHub != null)
        {
            mapHub.Invoke(null, new object?[] { app, hubT, pattern, null });
        }
    }

    private static Type? ResolveType(string fullName)
        => AppDomain.CurrentDomain.GetAssemblies()
              .Select(a => a.GetType(fullName, throwOnError: false, ignoreCase: false))
              .FirstOrDefault(t => t != null);
}
