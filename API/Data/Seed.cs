using System;
using System.Threading.Tasks;
using API.Models;
using API.Enums;
using API.Services.Argon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Data
{
    public static class Seed
    {
        public static async Task SeedAdminUser(AppDbContext context, IArgonHashing argonHashing, ILogger logger)
        {
            // Check if we already have any admin users
            if (await context.Users.AnyAsync(u => u.Role == Roles.Admin))
            {
                logger.LogInformation("Admin user already exists. Skipping admin seeding.");
                return;
            }
            
            logger.LogInformation("No admin user found. Creating default admin account...");
            
            try
            {
                // Create admin user
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@careemclone.com",
                    Role = Roles.Admin,
                    CreatedAt = DateTime.UtcNow
                };
                
                // Hash the admin password with Argon
                adminUser.PasswordHash = await argonHashing.HashPasswordAsync("Admin@123456");
                
                // Add admin to database
                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
                
                logger.LogInformation($"Admin user created successfully with ID: {adminUser.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding admin user");
                throw;
            }
        }
    }
}
