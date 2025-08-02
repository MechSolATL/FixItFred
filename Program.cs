// [Sprint1002_FixItFred] Fixed all namespace references to match actual project structure
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Controllers.Api;
using Wangkanai.Detection;
using Middleware;
using static Services.Admin.AutoRepairEngine;
using static Services.Admin.ComplianceReportService;
using static Services.Admin.ValidationSimulatorService;
using FixItFred.Extensions.Diagnostics;
using Services.Diagnostics;
using Services.Admin;
using Services;
using Services.System;
using Data;

var builder = WebApplication.CreateBuilder(args);

// 🔍 Inject FixItFred diagnostics logging for build environment and DI inspection
Diagnostics.FixItFredDiagnostics.StartupLogger.Log(builder);

// 🚀 Core service configuration and DI registration
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SeoService>(); // 📎 Handles dynamic SEO content per page
builder.Services.AddScoped<SystemDiagnosticsService>();
builder.Services.AddScoped<IRootCauseCorrelationEngine, RootCauseCorrelationEngine>();
builder.Services.AddScoped<IReplayEngineService, ReplayEngineService>();
builder.Services.AddScoped<ISmartAdminAlertsService, SmartAdminAlertsService>();
// [Sprint1002_FixItFred] Fixed to register correct AutoRepairEngine from Services.Admin
builder.Services.AddScoped<Services.Admin.AutoRepairEngine>();
builder.Services.AddScoped<ComplianceReportService>();
// [Sprint1002_FixItFred] Added missing service registrations to resolve DI errors
builder.Services.AddScoped<Services.Admin.TechnicianAuditService>();
builder.Services.AddScoped<Services.Admin.PermissionService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
#pragma warning disable CS0618
builder.Services.AddScoped<ValidationSimulatorService>(); // ⚠️ [Obsolete] — monitor usage
#pragma warning restore CS0618

// 🚀 Sprint121 - Revitalize SaaS Module Registration
builder.Services.AddScoped<Revitalize.Services.ITenantService, Revitalize.Services.TenantService>();
builder.Services.AddScoped<Revitalize.Services.IServiceRequestService, Revitalize.Services.ServiceRequestService>();
builder.Services.AddScoped<Revitalize.Services.IRevitalizeConfigService, Revitalize.Services.RevitalizeConfigService>();
builder.Services.AddScoped<Revitalize.Services.IRevitalizeSeoService, Revitalize.Services.RevitalizeSeoService>();
builder.Services.AddScoped<Revitalize.Services.Nova.INovaRevitalizePlanningService, Revitalize.Services.Nova.NovaRevitalizePlanningService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ✅ Runtime middleware pipeline
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

app.Run();

public class AutoRepairEngine
{
    public Task<bool> DetectCorruptionAsync(string dataSetId) => Task.FromResult(true);
    public Task<bool> RunAutoRepairAsync(string technicianId) => Task.FromResult(true);
}

public class ComplianceReportService {}

public class ValidationSimulatorService {}

public interface ISmartAdminAlertsService
{
    Task TriggerAlertsAsync(string systemId);
    Task AcknowledgeAlertAsync(string alertId);
    Task<List<string>> GetActiveAlertsAsync();
}

public class LeaderboardService : ILeaderboardService {}

public interface ILeaderboardService {}

// Make Program class public for testing with WebApplicationFactory
public partial class Program { }
