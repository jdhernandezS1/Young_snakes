using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Young_snakes.Data;
using Young_snakes.Models.Auth;
using DotNetEnv;
// using Young_snakes.Services;
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// builder.Services.AddSingleton<CloudinaryService>();

builder.Services.AddDbContextFactory<ApplicationDbContext>(
    options => options.UseNpgsql(
        Environment.GetEnvironmentVariable("DATABASE")
    )
);


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// using (var scope = app.Services.CreateScope())
// {
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//     string[] roles = { "SuperAdmin", "TeamUser" };

//     foreach (var role in roles)
//     {
//         if (!await roleManager.RoleExistsAsync(role))
//             await roleManager.CreateAsync(new IdentityRole(role));
//     }
//     var services = scope.ServiceProvider;
//     await DataSeeder.SeedAsync(services);
// }

app.Run();
