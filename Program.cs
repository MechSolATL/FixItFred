namespace MVP_Core.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
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

        // Run Sprint122_CertumDNSBypass tests if requested
        if (args.Length > 0 && args[0] == "--test")
        {
            Console.WriteLine("Running Sprint122_CertumDNSBypass validation tests...\n");
            TestRunner.RunTests();
            return;
        }

        app.Run();
    }
}