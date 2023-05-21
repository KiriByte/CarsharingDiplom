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
builder.Services.AddSingleton<CarTelemetryService>(provider =>
{
    return new CarTelemetryService(telemetryAppName);
});

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

    List<string> users = new List<string>()
    {
        "kmisyuro@gmail.com", "kmisyuro@gmail.com1", "kmisyuro@gmail.com2", "kmisyuro@gmail.com3",
        "kmisyuro@gmail.com4",
        "kmisyuro@gmail.com5", "kmisyuro@gmail.com6", "kmisyuro@gmail.com7", "kmisyuro@gmail.com8",
        "kmisyuro@gmail.com9"
    };

    foreach (var user in users)
    {
        if (await userManager.FindByEmailAsync(user) == null)
        {
            var newuser = new ApplicationUser
            {
                UserName = user,
                Email = user,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(newuser, "76sqon9Z!");

            var isInRole = await userManager.IsInRoleAsync(newuser, "user");
            if (!isInRole)
            {
                await userManager.AddToRoleAsync(newuser, "user");
            }
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