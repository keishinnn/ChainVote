using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChainVote.Data;
using ChainVote.Models.Identity;
using ChainVote.SeedData;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ✅ Database connection
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // ✅ Add Identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // ✅ Add MVC
        builder.Services.AddControllersWithViews();

        // ✅ Register the background service
        builder.Services.AddHostedService<ElectionStatusUpdaterService>();

        var app = builder.Build();

        // ✅ Seed Roles
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await IdentityDataInitializer.SeedRolesAsync(roleManager);
        }

        // ✅ Middleware
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=DefaultView}/{action=Index}/{id?}");

        await app.RunAsync();
    }
}
