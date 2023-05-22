using CarsharingProject.Data;
using CarsharingProject.Models;
using CarsharingProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       //throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var sqlPassword = Environment.GetEnvironmentVariable("SA_PASSWORD");
var sqlServer = Environment.GetEnvironmentVariable("SQL_SERVER");
var connectionString = $"Server={sqlServer};Database=Carsharing;User=sa;Password={sqlPassword};TrustServerCertificate=yes;";
//var connectionString = $"Server=192.168.1.10;Database=CarsharingProject;User=sa;Password=76sqon9Z;TrustServerCertificate=yes;";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options
        => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//string telemetryAppName = Environment.GetEnvironmentVariable("TELEMETRY_SERVER");
string telemetryAppName = Environment.GetEnvironmentVariable("TELEMETRY_SERVER");
builder.Services.AddSingleton<CarTelemetryService>(_ => new CarTelemetryService(telemetryAppName));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    List<string> roles = new()
    {
        "admin", "employee", "user", "verifiedUser"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var addrole = new IdentityRole(role);
            await roleManager.CreateAsync(addrole);
        }
    }

    #region AddUsers

    if (await userManager.FindByEmailAsync("admin@test.com") == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin@test.com",
            Email = "admin@test.com",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "Test1!");

        var isInRole = await userManager.IsInRoleAsync(adminUser, "admin");
        if (!isInRole)
        {
            await userManager.AddToRoleAsync(adminUser, "admin");
        }
    }

    #endregion


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();

    app.Run();
}