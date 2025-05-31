using API.Data;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Argon;
using API.Services.CmsServiceFolder;
using API.Services.EmailService;
using API.Services.OrderServiceFolder; 
using API.Services.OtpService;
using API.Services.TokenServiceFolder;
using API.Services.TokenServiceFolder.AuthService;
using API.Services.TokenServiceFolder.AuthServiceFolder;
using API.UoW;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Add Swagger documentation using our extension method
            services.AddSwaggerDocumentation();
            
            // Add CORS services
            services.AddCorsServices();
            
            // Add Authentication
            services.AddJwtAuthentication(config);
            
            // Add Authorization
            services.AddAuthorization();
            
            // Add Database Context
            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(config.GetConnectionString("DefaultConnection")));


            // Services Registration
            services.AddHttpContextAccessor();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IArgonHashing, ArgonHashing>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IDishManagementService, DishManagementService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IUserManagementService, UserManagementService>();
            services.AddTransient<IRestaurantManagement, RestaurantManagement>();
            services.AddTransient<ICategoryManagementService, CategoryManagementService>();            services.AddTransient<ICloudinaryService, CloudinaryService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IOtpService, OtpService>();

            // Reposiries Registration
            services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            services.AddScoped<IDishRepository, DishRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            
            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            return services;
        }
        
        // CORS Configuration
        public static IServiceCollection AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                // General policy for all origins
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
                
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    // In development, allow any origin for easier debugging
                    policy.SetIsOriginAllowed(origin => true) // Allow any origin
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });

                options.AddPolicy("AllowFlutterWeb", builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "http://localhost", "http://127.0.0.1:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            
            return services;
        }
        
        // JWT Authentication Configuration
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured"))),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured"),
                    ValidAudience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured")
                };
            });
            
            return services;
        }
    }
}