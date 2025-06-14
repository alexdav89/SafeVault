using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeVault.Data;
using SafeVault.Models;

var builder = WebApplication.CreateBuilder(args);

// Use Sqlite database
builder.Services.AddDbContext<SafeVaultDbContext>(options =>
    options.UseSqlite("Data Source=safevault.db"));

// Add Identity Services
builder.Services.AddIdentity<UserModel, IdentityRole>()
    .AddEntityFrameworkStores<SafeVaultDbContext>()
    .AddDefaultTokenProviders();

// Add Authentication & Authorization
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed Users & Roles
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await DataSeeder.SeedUsersAndRolesAsync(serviceProvider);
}

// Configure Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();