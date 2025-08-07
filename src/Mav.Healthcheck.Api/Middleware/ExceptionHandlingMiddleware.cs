using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Mav.Healthcheck.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private const string DefaultErrorMessage = "An internal server error has occurred.";

    private readonly RequestDelegate next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandlingExceptionAsync(context, ex);
        }
    }

    private async Task HandlingExceptionAsync(HttpContext context, Exception ex)
    {
        logger.LogError(ex, $"Error processing request: {context.Request.Method} {context.Request.Path} {context.Request.Protocol}; Error MessageL {ex.Message}");

        var (statusCode, problemDetails) = ex switch
        {
            // Add Custom Exception types
            _ => MapException(ex, HttpStatusCode.InternalServerError, DefaultErrorMessage)
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private static (HttpStatusCode, ProblemDetails) MapException(Exception ex, HttpStatusCode statusCode, string errorMessage) =>
        (statusCode, new ProblemDetails
        {
            Status = (int)statusCode,
            Detail = errorMessage,
            Type = ex.GetType().Name,
            Title = ex.GetType().Name
        });
}
