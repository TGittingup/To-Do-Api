using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoAPI2.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure detailed logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

try
{
    // Configure database context
    builder.Services.AddDbContext<TodoContext>(options =>
    {
        // Always use SQLite, even in Azure
        options.UseSqlite("Data Source=todo.db");
    });

    // Add services
    builder.Services.AddControllers();

    // Enable CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // Add Swagger for API documentation
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "TodoAPI2",
            Version = "v1",
            Description = "Simple Todo API built with ASP.NET Core and EF Core"
        });
    });

    var app = builder.Build();

    // Enable Swagger in all environments for now
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoAPI2 v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root "/"
    });

    // Redirect HTTP to HTTPS
    app.UseHttpsRedirection();

    // Use CORS
    app.UseCors("AllowAll");

    // Use authorization (if needed)
    app.UseAuthorization();

    // Map controller routes
    app.MapControllers();

    // Initialize the database
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<TodoContext>();
            var logger = services.GetRequiredService<ILogger<Program>>();
            
            logger.LogInformation("Attempting to ensure database exists and is up to date");
            context.Database.EnsureCreated();
            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while initializing the database");
            throw; // Rethrow to show the error in Azure
        }
    }

    app.Run();
}
catch (Exception ex)
{
    // Log any startup errors
    var logger = LoggerFactory.Create(config =>
    {
        config.AddConsole();
        config.AddDebug();
    }).CreateLogger<Program>();

    logger.LogError(ex, "Application startup failed");
    throw; // Rethrow to show error in Azure
}
