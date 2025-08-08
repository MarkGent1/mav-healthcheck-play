using Amazon.S3;
using Mav.Healthcheck.Infrastructure.Storage.Configuration;
using Mav.Healthcheck.Infrastructure.Storage.Factories;
using Mav.Healthcheck.Infrastructure.Storage.Factories.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mav.Healthcheck.Infrastructure.Storage.Setup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddS3ClientDependenciesWithFactory(this IServiceCollection services)
    {
        services.AddSingleton<IS3ClientFactory, S3ClientFactory>();

        return services;
    }

    public static IServiceCollection AddS3ClientDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var storageConfiguration = configuration.GetSection(nameof(StorageConfiguration))
            .Get<StorageConfiguration>()!;
        services.AddSingleton(storageConfiguration);

        if (configuration["AWS:OverrideServiceURL"] != null)
        {
            services.AddSingleton<IAmazonS3>(sp =>
            {
                var config = new AmazonS3Config
                {
                    ServiceURL = configuration["AWS:ServiceURL"],
                    AuthenticationRegion = configuration["AWS:Region"],
                    ForcePathStyle = true,
                    UseHttp = true
                };
                return new AmazonS3Client(config);
            });
        }
        else
        {
            services.AddAWSService<IAmazonS3>();
        }

        return services;
    }
}
