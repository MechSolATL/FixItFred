// ------------------------
// Program.cs
// ------------------------

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Data;
using MVP_Core.Data.Seeders;
using MVP_Core.Services;
using MVP_Core.Services.Email;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 🔧 Load Configuration
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        Console.WriteLine("\u2705 Using Local Development Connection String");

        // 🧱 MVC & Razor
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

        // ⚡ Blazor + SignalR
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSignalR();

        // 🧩 Application Services (Scoped DI)
        builder.Services.AddScoped<ISeoService, SeoService>();
        builder.Services.AddScoped<SeoService>(); // 👈 Register concrete type

        builder.Services.AddScoped<ContentService>();
        builder.Services.AddScoped<EmailService>();
        builder.Services.AddScoped<QuestionService>();
        builder.Services.AddScoped<BackgroundImageService>();
        builder.Services.AddScoped<SmsService>();
        builder.Services.AddScoped<ProfileReviewService>();
        builder.Services.AddScoped<MVP_Core.Services.AuditLogger>();

        builder.Services.AddHostedService<MVP_Core.Services.BackupReminderService>();

        // 🗃️ EF Core DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // 🛎️ Session Setup
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // 🔐 Authentication
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
        });

        // 📋 Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
            options.SingleLine = false;
        });
        builder.Logging.AddDebug();

        var app = builder.Build();

        // 🌐 Global Error Handling
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        // 🛡️ Basic Security Headers
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self';");
            await next();
        });

        // 🚦 Middleware Pipeline
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStatusCodePagesWithReExecute("/Error");

        // 📍 Endpoint Mapping
        app.MapRazorPages();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        // 🌱 Initial Database Seeding
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
            DatabaseSeeder.Seed(db);
            PagesSeeder.Seed(db);

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "MSA-Atlanta.jpg");
            ImageSeeder.SeedBackgroundImage(db, imagePath);
        }

        app.Run();
    }
}
