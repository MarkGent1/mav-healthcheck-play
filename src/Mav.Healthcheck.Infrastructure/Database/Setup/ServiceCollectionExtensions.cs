using Mav.Healthcheck.Infrastructure.Database.Factories;
using Mav.Healthcheck.Infrastructure.Database.Factories.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mav.Healthcheck.Infrastructure.Database.Setup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoConfig>(configuration.GetSection("Mongo"));
        services.AddSingleton<IMongoDbClientFactory, MongoDbClientFactory>();

        return services;
    }
}
