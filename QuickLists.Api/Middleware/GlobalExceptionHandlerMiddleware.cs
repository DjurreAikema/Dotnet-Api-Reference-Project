using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace QuickLists.Api.Middleware;

public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occured while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Set response content type
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

        // Create problem details response
        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7807#section-6.6.1", // TODO What does this actually mean?
            Title = "An error occurred while processing you request",
            Status = (int) HttpStatusCode.InternalServerError,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, options);

        return context.Response.WriteAsync(json);
    }
}

public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}