using Microsoft.EntityFrameworkCore;
using QuickLists.Api.Endpoints;
using QuickLists.Core.Interfaces;
using QuickLists.Infrastructure.Data;
using QuickLists.Infrastructure.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IChecklistRepository, ChecklistRepository>();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

// Map Endpoints
app.MapChecklistEndpoints();
app.MapChecklistItemEndpoints();

// TODO: Add authentication middleware when implementing auth
// app.UseAuthentication();
// app.UseAuthorization();

app.Run();