using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeVault.Models;
using System;
using System.Threading.Tasks;

namespace SafeVault.Data
{
    public class DataSeeder
    {
        public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<UserModel>>();

            // Ensure roles exist
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed admin user
            var adminUser = new UserModel
            {
                UserName = "admin@safevault.com",
                Email = "admin@safevault.com",
                Role = "Admin",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Password!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminUser.Role);
                }
            }

            // Seed regular user
            var regularUser = new UserModel
            {
                UserName = "user@safevault.com",
                Email = "user@safevault.com",
                Role = "User",
                EmailConfirmed = true
            };

            if (await userManager.FindByEmailAsync(regularUser.Email) == null)
            {
                var result = await userManager.CreateAsync(regularUser, "Password!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, regularUser.Role);
                }
            }
        }
    }
}