using Mav.Healthcheck.Infrastructure.Database.Factories;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Mav.Healthcheck.Infrastructure.Database.Setup;

public class MongoDbHealthCheck : IHealthCheck
{
    private readonly IMongoDbClientFactory _mongoDbClientFactory;
    private readonly IMongoClient _mongoClient;

    public MongoDbHealthCheck(IMongoDbClientFactory mongoDbClientFactory)
    {
        _mongoDbClientFactory = mongoDbClientFactory;
        _mongoClient = _mongoDbClientFactory.GetClient();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _mongoClient.ListDatabaseNamesAsync(cancellationToken);
            return HealthCheckResult.Healthy("MongoDB is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB health check failed.", ex);
        }
    }
}
