using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Young_snakes.Models.Auth;
using DotNetEnv;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using QuestPDF.Infrastructure;

// using Young_snakes.Services;

QuestPDF.Settings.License = LicenseType.Community;
Env.Load();


var builder = WebApplication.CreateBuilder(args);
var database = Environment.GetEnvironmentVariable("DATABASE");
string cloud = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
string my_api_key = Environment.GetEnvironmentVariable("API_KEY");
string my_api_secret = Environment.GetEnvironmentVariable("API_SECRET");
string my_cloud_name = Environment.GetEnvironmentVariable("CLOUD_NAME");
// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(database));
builder.Services.AddScoped<IImageUploadService, CloudinaryUploadService>();

Account account = new Account(
    my_cloud_name,
    my_api_key,
    my_api_secret);

Cloudinary cloudinaryInstance = new Cloudinary(account);

cloudinaryInstance.Api.Secure = true;

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

// Registramos Cloudinary como un servicio único (Singleton) en la aplicación
builder.Services.AddSingleton(cloudinaryInstance);
// ==========================================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "SuperAdmin", "TeamUser" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
    var services = scope.ServiceProvider;
    await DataSeeder.SeedAsync(services);
    // }
}
app.Run();
