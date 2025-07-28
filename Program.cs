using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using MVP_Core.Controllers.Api;
using MVP_Core.Services.Email;
using MVP_Core.Services; // Only use this for AuditLogger and BackupReminderService
using Wangkanai.Detection;
using MVP_Core.Middleware;
using MVP_Core.Helpers;
using MVP_Core.Services.Admin;
using MVP_Core.Services.Dispatch;
using MVP_Core.Services.System;
using MVP_Core.Services.FollowUp;
using MVP_Core.Data.Seeders;
using MVP_Core.Services.Loyalty;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

// Add services
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddRazorPages()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });
builder.Services.AddDetection();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();

builder.Services.AddScoped<ISeoService, SeoService>();
builder.Services.AddScoped<ContentService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<BackgroundImageService>();
builder.Services.AddScoped<SmsService>();
builder.Services.AddScoped<ProfileReviewService>();
builder.Services.AddScoped<AuditLogger>();
builder.Services.AddScoped<EmailVerificationService>();
builder.Services.AddHostedService<BackupReminderService>();
builder.Services.AddHostedService<MVP_Core.Services.SlaEscalationService>();
builder.Services.AddHostedService<SLAMonitorService>(provider =>
{
    var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
    var notificationDispatchEngine = provider.GetRequiredService<NotificationDispatchEngine>();
    var dispatcherService = provider.GetRequiredService<DispatcherService>();
    return new SLAMonitorService(scopeFactory, notificationDispatchEngine, dispatcherService);
});
builder.Services.AddScoped<ITechnicianService, TechnicianService>();
builder.Services.AddScoped<MVP_Core.Services.IDeviceResolver, MVP_Core.Services.DeviceResolver>();
builder.Services.AddScoped<MVP_Core.Services.INotificationService, MVP_Core.Services.NotificationService>();
builder.Services.AddScoped<MVP_Core.Services.TechnicianProfileService>();
builder.Services.AddScoped<MVP_Core.Services.Reports.TechnicianReportService>();
builder.Services.AddScoped<ITechnicianProfileService, TechnicianProfileService>();
builder.Services.AddScoped<DispatcherService>();
builder.Services.AddScoped<NotificationDispatchEngine>();
builder.Services.AddScoped<ServiceRequestService>();
builder.Services.AddScoped<TechnicianFeedbackService>(); // FixItFred: Sprint 30B - Register TechnicianFeedbackService required by DispatcherService
builder.Services.AddScoped<INotificationHelperService, NotificationHelperService>();
builder.Services.AddScoped<MVP_Core.Services.FollowUp.FollowUpAIService>();
builder.Services.AddScoped<TechnicianPayService>();
builder.Services.AddScoped<CertificationService>();
builder.Services.AddScoped<LoyaltyRewardService>();
builder.Services.AddScoped<LoyaltyRewardEngine>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<CustomerPortalService>();
builder.Services.AddScoped<SkillsTrackerService>();
builder.Services.AddScoped<MVP_Core.Services.Admin.AssignmentScoringEngine>();
builder.Services.AddScoped<OfflineZoneTracker>();
builder.Services.AddScoped<AutoPrepEngine>();
builder.Services.AddScoped<SyncAnalyticsService>();
builder.Services.AddScoped<SyncIncentiveEngine>();
builder.Services.AddScoped<SystemDiagnosticsService>();
builder.Services.AddScoped<Services.Admin.AutoRepairEngine>();
builder.Services.AddScoped<Services.Dispatch.SlaDriftAnalyzerService>();
builder.Services.AddScoped<Services.Diagnostics.RootCauseCorrelationEngine>();
builder.Services.AddScoped<Services.Storage.StorageMonitorService>();
builder.Services.AddScoped<Services.Admin.ComplianceReportService>();
builder.Services.AddScoped<Services.Admin.SmartAdminAlertsService>();
builder.Services.AddScoped<Services.Admin.ScheduledMaintenanceEngine>();
builder.Services.AddScoped<Services.Admin.AdminDigestMailerService>();
builder.Services.AddScoped<Services.Admin.ValidationSimulatorService>();
builder.Services.AddScoped<MVP_Core.Services.Admin.TechnicianBehaviorAnalyzerService>(); // Sprint 71.0: Register behavior analyzer
builder.Services.AddScoped<SanityShieldService>();
builder.Services.AddScoped<IncidentCompressionService>();
builder.Services.AddScoped<DelayMatrixService>();
builder.Services.AddScoped<TrustCascadeEngine>();
builder.Services.AddScoped<AutoEscalationEngine>();
builder.Services.AddScoped<InactivityHeatmapAgent>();
builder.Services.AddScoped<GhostDelayAuditor>();
builder.Services.AddScoped<MVP_Core.Services.Admin.OvertimeDefenseService>();
builder.Services.AddScoped<MVP_Core.Services.Admin.GeoBreakValidatorService>();
builder.Services.AddScoped<MVP_Core.Services.Admin.IdleSessionMonitorService>();
builder.Services.AddScoped<RewardTriggerService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SessionTracker>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
    options.SingleLine = false;
});
builder.Logging.AddDebug();

builder.Services.Configure<MVP_Core.Services.Config.LoadBalancingConfig>(
    builder.Configuration.GetSection("LoadBalancing"));

// Register DataProtection for sensitive note encryption
builder.Services.AddDataProtection();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseDetection();
app.UseSecurityHeaders();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePagesWithReExecute("/Error");

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapBlazorHub();
app.MapHub<MVP_Core.Hubs.RequestHub>("/hubs/requests");
app.MapHub<MVP_Core.Hubs.ETAHub>("/etahub");
// Sprint 41.3 – Real-Time Message Broadcast
app.MapHub<MVP_Core.Hubs.JobMessageHub>("/hubs/jobmessages");
app.MapHub<MVP_Core.Hubs.RewardNotificationHub>("/hubs/rewardnotifications");
app.MapHub<MVP_Core.Hubs.NotificationHub>("/notificationHub"); // Register NotificationHub for SignalR

// Configure SensitiveNoteEncryptor with DataProtection
SensitiveNoteEncryptor.Configure(app.Services.GetRequiredService<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>());

// DB Seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    MVP_Core.Data.Seeders.DatabaseSeeder.Seed(db, 0); // FixItFred: Use explicit discard value instead of _
    MVP_Core.Data.Seeders.PagesSeeder.Seed(db);

#if DEBUG
    // FixItFred: Sprint 30D.3 — Trigger Live QA Test Run for ScheduleQueue/Dispatcher/SignalR 2024-07-25
    var dispatcherService = scope.ServiceProvider.GetRequiredService<DispatcherService>();
    var dispatchEngine = scope.ServiceProvider.GetRequiredService<NotificationDispatchEngine>();
    MVP_Core.Data.Seeders.DatabaseSeeder.SeedTestServiceRequests(db, dispatcherService, dispatchEngine);
#endif

    try
    {
        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "MSA-Atlanta.jpg");
        MVP_Core.Data.Seeders.ImageSeeder.SeedBackgroundImage(db, imagePath);
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
        logger.LogInformation("Background image seeded successfully.");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
        logger.LogError(ex, "Error seeding background image.");
    }

    ClockInTestSeeder.Seed(db);
}

app.Run();
