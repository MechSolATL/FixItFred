using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Services;
using Microsoft.AspNetCore.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuration Settings
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        // Resolve Environment
        var environment = builder.Environment.EnvironmentName ?? "Development";
        Console.WriteLine($"Resolved Environment: {environment}");

        // Get Connection String
        var connectionStringKey = environment.Equals("Development", StringComparison.OrdinalIgnoreCase) ? "Development" : "Production";
        var connectionString = builder.Configuration.GetSection("ConnectionStrings")[connectionStringKey];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Database connection string for '{connectionStringKey}' is missing.");
        }
        Console.WriteLine($"Using Connection String: {connectionString}");

        // Register Services
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        // Core Services Needed for MVP
        builder.Services.AddScoped<SEOService>();
        builder.Services.AddScoped<ContentService>();
        builder.Services.AddScoped<EmailService>();
        builder.Services.AddScoped<QuestionService>();

        // Database Context
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Session Configuration
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // Authorization Policies
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));

        // HSTS Configuration
        builder.Services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });

        // CORS Configuration (Optional)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", builder =>
                builder.WithOrigins("https://example.com")
                       .AllowAnyHeader()
                       .AllowAnyMethod());
        });

        // Enhanced Logging Configuration
        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
            options.SingleLine = false;
        });
        builder.Logging.AddDebug();

        // Build Application
        var app = builder.Build();

        // Middleware Configuration
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
            }
        });
        app.UseSession();

        app.Use((context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.XFrameOptions = "DENY";
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'");
            return next();
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // Exception Handling
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(exceptionHandlerPathFeature?.Error, "Unhandled exception occurred.");

                if (context.Response.StatusCode == 404)
                {
                    context.Response.Redirect("/Home/NotFound");
                }
                else
                {
                    context.Response.Redirect("/Home/Error");
                }

                return Task.CompletedTask;
            });
        });

        // CORS Middleware
        app.UseCors("AllowSpecificOrigins");

        // Map Razor Pages and Routes
        app.MapRazorPages();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
