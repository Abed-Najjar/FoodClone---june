using API.Data;
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
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}