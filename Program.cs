using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using MVP_Core.Controllers.Api;
using MVP_Core.Services.Email;
using Wangkanai.Detection;
using MVP_Core.Middleware;
using MVP_Core.Helpers;
using MVP_Core.Services.Dispatch;
using MVP_Core.Services.FollowUp;
using MVP_Core.Data.Seeders;
using MVP_Core.Services.Loyalty;
using static Services.Admin.AutoRepairEngine;
using static Services.Admin.ComplianceReportService;
using static Services.Admin.ValidationSimulatorService;
using FixItFred.Extensions.Diagnostics;
using Services.Diagnostics;
using MVP_Core.Services.Diagnostics;
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
builder.Services.AddScoped<Data.Models.AutoRepairEngine>();
builder.Services.AddScoped<ComplianceReportService>();
#pragma warning disable CS0618
builder.Services.AddScoped<ValidationSimulatorService>(); // ⚠️ [Obsolete] — monitor usage
#pragma warning restore CS0618
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
