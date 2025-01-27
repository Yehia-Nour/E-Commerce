using BestStoreMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace BestStoreMVC.Services
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager,
            RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("userManager or roleManager is null => exit");
                return;
            }

            // Check for roles
            var roles = new[] { "admin", "seller", "client" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    Console.WriteLine($"{role} role is not defined and will be created");
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Check for admin users
            var adminUsers = await userManager.GetUsersInRoleAsync("admin");
            if (adminUsers.Any())
            {
                Console.WriteLine("Admin user already exists => exit");
                return;
            }

            // Create the admin user
            var user = new ApplicationUser()
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                CreatedAt = DateTime.Now,
            };

            const string initialPassword = "admin123";

            var result = await userManager.CreateAsync(user, initialPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("Admin user created successfully! Please update the initial password!");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine($"Initial password: {initialPassword}");
            }
        }
    }
}
