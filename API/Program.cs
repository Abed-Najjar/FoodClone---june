using API.Data;
using API.Extensions;
using API.Services.Argon;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add CORS services - unified configuration
builder.Services.AddCors(options =>
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

// Add authentication configuration
builder.Services.AddAuthentication(options =>
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
            builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured"))),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured"),
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured")
    };
});

// Make sure you add authentication before authorization
builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Add application services from extension methods
builder.Services.AddApplicationServices(builder.Configuration);


// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Careem Clone API", 
        Version = "v1",
        Description = "A simple Careem clone API",
        Contact = new OpenApiContact
        {
            Name = "API Support"
        }
    });

    // Define the JWT bearer auth scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Make sure Swagger UI requires a Bearer token
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Register the CloudinaryService as a singleton
builder.Services.AddSingleton<API.Services.CmsServiceFolder.CloudinaryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Careem Clone API v1");
        c.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

// Enable CORS - this must be called before authentication middleware
app.UseCors("AllowAngularApp"); // Use the Angular policy for all web clients

// Add authentication middleware before authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var argonHashing = services.GetRequiredService<IArgonHashing>();
        var logger = services.GetRequiredService<ILogger<Program>>();
          // Apply any pending migrations
        await context.Database.MigrateAsync();
        
        // Seed data (admin user and restaurants)
        await Seed.SeedData(context, argonHashing, logger);
        
        logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database seeding");
    }
}

app.Run();

