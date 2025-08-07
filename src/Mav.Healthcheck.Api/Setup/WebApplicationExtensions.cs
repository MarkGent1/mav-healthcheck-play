using Mav.Healthcheck.Api.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;

namespace Mav.Healthcheck.Api.Setup;

public static class WebApplicationExtensions
{
    public static void ConfigureRequestPipeline(this WebApplication app)
    {
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        var applicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        applicationLifetime.ApplicationStarted.Register(() =>
            logger.LogInformation("{applicationName} started", env.ApplicationName));
        applicationLifetime.ApplicationStopping.Register(() =>
            logger.LogInformation("{applicationName} stopping", env.ApplicationName));
        applicationLifetime.ApplicationStopped.Register(() =>
            logger.LogInformation("{applicationName} stopped", env.ApplicationName));

        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new Asp.Versioning.ApiVersion(1.0))
            .ReportApiVersions()
            .Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.MapHealthChecks("/health", new HealthCheckOptions() { 
            Predicate = _ => true,
            ResponseWriter = WriteResponse,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        }).WithApiVersionSet(versionSet).IsApiVersionNeutral();

        app.MapGet("/", () => "Alive!").WithApiVersionSet(versionSet).IsApiVersionNeutral();

        app.UseHttpsRedirection();
        app.MapControllers();
    }

    private static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status",
                    healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description",
                    healthReportEntry.Value.Description);
                jsonWriter.WriteStartObject("data");

                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}
