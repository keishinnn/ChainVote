using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;
using ChainVote.Models.Identity;
using ChainVote.SeedData;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create a new web application builder instance
        var builder = WebApplication.CreateBuilder(args);

        // Configure Entity Framework to use SQL Server with the provided connection string
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register ASP.NET Core Identity with ApplicationUser and IdentityRole
        // Disable confirmation requirement on sign-in
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()   // Store identity data in the configured DbContext
        .AddDefaultTokenProviders();                        // Adds token providers for password reset, etc.

        // Register MVC services to support controllers and views
        builder.Services.AddControllersWithViews();

        // Register custom background service to handle election status updates
        builder.Services.AddHostedService<ElectionStatusUpdaterService>();

        // Build the application
        var app = builder.Build();

        // Seed default roles into the database at application startup
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await IdentityDataInitializer.SeedRolesAsync(roleManager);
        }

        // Middleware pipeline configuration

        // Redirect HTTP requests to HTTPS
        app.UseHttpsRedirection();

        // Serve static files (e.g., CSS, JS, images)
        app.UseStaticFiles();

        // Enable routing to match URLs to controller actions
        app.UseRouting();

        // Enable authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Configure default route pattern for controllers
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=DefaultView}/{action=Index}/{id?}");

        // Run the application asynchronously
        await app.RunAsync();
    }
}
