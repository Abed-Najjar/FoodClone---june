using API.Data;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Argon;
using API.Services.TokenServiceFolder;
using API.Services.TokenServiceFolder.AuthService;
using API.Services.TokenServiceFolder.AuthServiceFolder;
using API.Services.UserServiceFolder;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            
            // Add your application services here
            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(config.GetConnectionString("DefaultConnection")));


            // Service Registration
            services.AddScoped<ITokenService, TokenService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IArgonHashing, ArgonHashing>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDishManagementService, DishManagementService>();
            services.AddScoped<IOrderManagementService, OrderManagementService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IRestaurantManagement, RestaurantManagement>();
            services.AddScoped<ICategoryManagementService, CategoryManagementService>();
            services.AddScoped<IUserService, UserService>();


            // Register repositories
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<IDishRepository, DishRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            

            return services;
        }
    }
}