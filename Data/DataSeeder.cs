using Microsoft.AspNetCore.Identity;
using Young_snakes.Models.Auth;

namespace Young_snakes.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "SuperAdmin", "TeamUser" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // SuperAdmin
            if (await userManager.FindByEmailAsync("admin@youngsnakes.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@youngsnakes.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "SuperAdmin");
            }

            // TeamUser
            if (await userManager.FindByEmailAsync("team@youngsnakes.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "teamuser",
                    Email = "team@youngsnakes.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Team123!");
                await userManager.AddToRoleAsync(user, "TeamUser");
            }
        }
    }
}