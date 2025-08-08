using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Mav.Healthcheck.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mav.Healthcheck.Infrastructure.Messaging.Setup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceBusSenderDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusSenderConfiguration = configuration.GetSection(nameof(ServiceBusSenderConfiguration))
            .Get<ServiceBusSenderConfiguration>()!;
        services.AddSingleton(serviceBusSenderConfiguration);

        if (configuration["AWS:OverrideServiceURL"] != null)
        {
            services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
            {
                var config = new AmazonSimpleNotificationServiceConfig
                {
                    ServiceURL = configuration["AWS:ServiceURL"],
                    AuthenticationRegion = configuration["AWS:Region"],
                    UseHttp = true
                };
                return new AmazonSimpleNotificationServiceClient(config);
            });
        }
        else
        {
            services.AddAWSService<IAmazonSimpleNotificationService>();
        }

        return services;
    }

    public static IServiceCollection AddServiceBusReceiverDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusReceiverConfiguration = configuration.GetSection(nameof(ServiceBusReceiverConfiguration))
            .Get<ServiceBusReceiverConfiguration>()!;
        services.AddSingleton(serviceBusReceiverConfiguration);

        if (configuration["AWS:OverrideServiceURL"] != null)
        {
            services.AddSingleton<IAmazonSQS>(sp =>
            {
                var config = new AmazonSQSConfig
                {
                    ServiceURL = configuration["AWS:ServiceURL"],
                    AuthenticationRegion = configuration["AWS:Region"],
                    UseHttp = true
                };
                var credentials = new Amazon.Runtime.BasicAWSCredentials("test", "test");
                return new AmazonSQSClient(credentials, config);
            });
        }
        else
        {
            services.AddAWSService<IAmazonSQS>();
        }

        return services;
    }
}
