using CarsharingProject.Data;
using CarsharingProject.Interfaces;
using CarsharingProject.Models;
using CarsharingProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? secretSqlPassword = builder.Configuration.GetValue<string?>("SA_PASSWORD");
string? secretSqlServer = builder.Configuration.GetValue<string?>("SQL_SERVER");
string? secretTelemetryServer = builder.Configuration.GetValue<string?>("TELEMETRY_SERVER");
if (secretSqlPassword != null)
{
    Environment.SetEnvironmentVariable("SA_PASSWORD", secretSqlPassword);
}

if (secretSqlServer != null)
{
    Environment.SetEnvironmentVariable("SQL_SERVER", secretSqlServer);
}

if (secretTelemetryServer != null)
{
    Environment.SetEnvironmentVariable("TELEMETRY_SERVER", secretTelemetryServer);
}

var sqlPassword = Environment.GetEnvironmentVariable("SA_PASSWORD");
var sqlServer = Environment.GetEnvironmentVariable("SQL_SERVER");
var connectionString =
    $"Server={sqlServer};Database=Carsharing;User=sa;Password={sqlPassword};TrustServerCertificate=yes;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<UserModel>(options
        => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

string telemetryAppName = Environment.GetEnvironmentVariable("TELEMETRY_SERVER");
builder.Services.AddSingleton<CarTelemetryService>(_ => new CarTelemetryService(telemetryAppName));

builder.Services.AddTransient<IBankCard, BankCardService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
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
        var adminUser = new UserModel()
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