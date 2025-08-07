using Mav.Healthcheck.Api.Middleware;

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

        app.MapHealthChecks("/health").WithApiVersionSet(versionSet).IsApiVersionNeutral();
        app.MapGet("/", () => "Alive!").WithApiVersionSet(versionSet).IsApiVersionNeutral();

        app.UseHttpsRedirection();
        app.MapControllers();
    }
}
