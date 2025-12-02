using FluentValidation;

namespace QuickLists.Api.Endpoints;

public static class DevEndpoints
{
    public static void MapDevEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dev")
            .WithTags("Development")
            .ExcludeFromDescription();

        group.MapGet("/simulate-500", () => { throw new Exception("Simulated server error"); });

        group.MapGet("/simulate-validation", () => { throw new ValidationException("Simulated validation failure"); });

        group.MapGet("/simulate-slow/{delayMs}", async (int delayMs) =>
        {
            await Task.Delay(delayMs);
            return Results.Ok(new {message = "Delayed response"});
        });
    }
}