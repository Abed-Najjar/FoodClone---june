using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;
using API.Enums;
using API.Services.Argon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Data
{    public static class Seed
    {
        public static async Task SeedData(AppDbContext context, IArgonHashing argonHashing, ILogger logger)
        {
            await SeedAdminUser(context, argonHashing, logger);
            await SeedRestaurants(context, logger);
        }
        
        public static async Task SeedRestaurants(AppDbContext context, ILogger logger)
        {
            // Check if we already have any restaurants
            if (await context.Restaurants.AnyAsync())
            {
                logger.LogInformation("Restaurants already exist. Skipping restaurant seeding.");
                return;
            }
            
            logger.LogInformation("No restaurants found. Creating sample restaurants...");
            
            try
            {
                // Create sample restaurants
                var restaurants = new List<Restaurant>
                {
                    new Restaurant
                    {
                        Name = "Burger King",
                        Description = "Famous for flame-grilled burgers",
                        Rating = 4.2m,
                        ReviewCount = 120,
                        Address = "123 Main St",
                        OpeningHours = "10:00 AM - 11:00 PM",
                        IsOpen = true,
                        LogoUrl = "https://via.placeholder.com/150x150?text=BurgerKing",
                        CoverImageUrl = "https://via.placeholder.com/1200x400?text=BurgerKing",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Restaurant
                    {
                        Name = "Pizza Hut",
                        Description = "Delicious pizzas with premium toppings",
                        Rating = 4.0m,
                        ReviewCount = 95,
                        Address = "456 Oak St",
                        OpeningHours = "11:00 AM - 10:00 PM",
                        IsOpen = true,
                        LogoUrl = "https://via.placeholder.com/150x150?text=PizzaHut",
                        CoverImageUrl = "https://via.placeholder.com/1200x400?text=PizzaHut",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Restaurant
                    {
                        Name = "Taco Bell",
                        Description = "Mexican-inspired fast food",
                        Rating = 3.8m,
                        ReviewCount = 78,
                        Address = "789 Elm St",
                        OpeningHours = "10:30 AM - 11:00 PM",
                        IsOpen = true,
                        LogoUrl = "https://via.placeholder.com/150x150?text=TacoBell",
                        CoverImageUrl = "https://via.placeholder.com/1200x400?text=TacoBell",
                        CreatedAt = DateTime.UtcNow
                    }
                };
                
                // Add restaurants to database
                await context.Restaurants.AddRangeAsync(restaurants);
                await context.SaveChangesAsync();
                
                logger.LogInformation($"{restaurants.Count} restaurants created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding restaurants");
                throw;
            }
        }
        
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
                    FirstName = "Admin",
                    LastName = "User",
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
