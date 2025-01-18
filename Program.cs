using MVP_Core.Data;
using MVP_Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using MVP_Core.Services;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuration Settings
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Services Registration
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        // Core Services Needed for MVP
        builder.Services.AddScoped<SEOService>();
        builder.Services.AddScoped<ContentService>();
        builder.Services.AddScoped<EmailService>();
        builder.Services.AddScoped<QuestionService>();  // Optional: Only if using questions

        // Database Context
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Session Configuration (if needed)
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Basic Logging
        builder.Logging.AddDebug();
        builder.Logging.AddEventSourceLogger();

        if (builder.Environment.IsDevelopment())
        {
            builder.Logging.AddConsole();
        }

        var app = builder.Build();

        // Middleware Configuration
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();

        app.UseRouting();
        app.UseAuthorization();

        // Map Razor Pages and Routes
        app.MapRazorPages();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
