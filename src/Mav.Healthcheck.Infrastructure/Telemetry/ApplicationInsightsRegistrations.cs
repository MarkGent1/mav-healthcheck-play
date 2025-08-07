using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mav.Healthcheck.Infrastructure.Telemetry;

public static class ApplicationInsightsRegistrations
{
    private const string ConfigKeyApplicationName = "ApplicationInsights:AppName";

    public static IServiceCollection AddApplicationInsightsApi(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationName = configuration.GetValue<string>(ConfigKeyApplicationName);

        services.AddApplicationInsightsTelemetry(options =>
        {
            options.EnableQuickPulseMetricStream = true;
            options.EnableAdaptiveSampling = false;
        })
            .Configure<TelemetryConfiguration>(config =>
            {
                config.DefaultTelemetrySink
                .TelemetryProcessorChainBuilder
                .UseAdaptiveSampling(2,
                    "Request;Exception;Dependency",
                    "Event")
                .Build();
            });

        return services;
    }

    public static IServiceCollection AddApplicationInsightsHostedService(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationName = configuration.GetValue<string>(ConfigKeyApplicationName);

        services.AddApplicationInsightsTelemetry(options =>
        {
            options.EnableQuickPulseMetricStream = true;
            options.EnableAdaptiveSampling = false;
        })
            .Configure<TelemetryConfiguration>(config =>
            {
                config.DefaultTelemetrySink
                .TelemetryProcessorChainBuilder
                .UseAdaptiveSampling(2,
                    "Request;Exception;Dependency",
                    "Event")
                .Build();
            });

        return services;
    }
}
