using API.Data;
using API.Extensions;
using API.Services.Argon;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add controller services
builder.Services.AddControllers();

// Add all application services using our consolidated extension method
builder.Services.AddApplicationServices(builder.Configuration);

// CloudinaryService is already registered as scoped in ApplicationServiceExtensions
// builder.Services.AddSingleton<API.Services.CmsServiceFolder.CloudinaryService>();

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

