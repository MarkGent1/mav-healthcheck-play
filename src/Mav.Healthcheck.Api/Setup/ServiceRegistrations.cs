using Asp.Versioning;
using Mav.Healthcheck.Infrastructure.Database.Setup;
using Mav.Healthcheck.Infrastructure.Messaging.Setup;
using Mav.Healthcheck.Infrastructure.Telemetry;

namespace Mav.Healthcheck.Api.Setup;

public static class ServiceRegistrations
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;

        services.AddLogging();

        services.AddApplicationInsightsApi(config);

        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddMongoDbDependencies(config);

        services.AddDefaultAWSOptions(config.GetAWSOptions());

        services.AddServiceBusSenderDependencies(config);

        services.AddServiceBusReceiverDependencies(config);

        services.ConfigureHealthChecks();
    }

    private static void ConfigureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<MongoDbHealthCheck>("mongodb", tags: ["db", "mongo"])
            .AddCheck<AwsSnsHealthCheck>("aws_sns", tags: ["aws", "sns"])
            .AddCheck<AwsSqsHealthCheck>("aws_sqs", tags: ["aws", "sqs"]);
    }
}
