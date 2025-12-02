using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuickLists.Api.Endpoints;
using QuickLists.Api.Middleware;
using QuickLists.Core.Behaviors;
using QuickLists.Core.Caching;
using QuickLists.Core.Interfaces;
using QuickLists.Infrastructure.Data;
using QuickLists.Infrastructure.Data.Repositories;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/quicklists-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting QuickLists API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog to the logging pipeline
    builder.Host.UseSerilog();

    // Add memory cache
    builder.Services.AddMemoryCache();

    // Add services to the container
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

    builder.Services.AddScoped<IChecklistRepository, ChecklistRepository>();
    builder.Services.AddScoped<IChecklistItemRepository, ChecklistItemRepository>();

    builder.Services.AddSingleton<ICacheKeyRegistry, CacheKeyRegistry>();
    builder.Services.AddSingleton<ICacheMetrics, CacheMetrics>();

    // Register MediatR with validation pipeline
    builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(typeof(QuickLists.Core.DTOs.ChecklistDto).Assembly);
        // Order matters. Behaviors run in the order they're added
        config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        config.AddOpenBehavior(typeof(CachingBehavior<,>));
    });

    // Register all FluentValidation validators
    builder.Services.AddValidatorsFromAssembly(typeof(QuickLists.Core.DTOs.ChecklistDto).Assembly);

    // Add problem details support
    builder.Services.AddProblemDetails();

    // Add CORS policy
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // Add minimal API services
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    app.UseGlobalExceptionHandler(); // Must be first

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Add request logging
    app.UseSerilogRequestLogging(options => { options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms"; });

    app.UseHttpsRedirection();
    app.UseCors("AllowAngular");

    // Map Endpoints
    if (app.Environment.IsDevelopment())
    {
        app.MapDevEndpoints();
    }

    app.MapChecklistEndpoints();
    app.MapChecklistItemEndpoints();

    // TODO: Add authentication middleware when implementing auth
    // app.UseAuthentication();
    // app.UseAuthorization();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make the implicit Program class accessible to tests
public partial class Program
{
}