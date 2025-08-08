using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Mav.Healthcheck.Api.Tests;

public class AppWebApplicationFactory : WebApplicationFactory<Program>
{
    private IHost? host;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("AWS__OverrideServiceURL", "Yes");
        Environment.SetEnvironmentVariable("AWS__ServiceURL", "http://localhost:4566");
        Environment.SetEnvironmentVariable("Mongo__DatabaseUri", "mongodb://127.0.0.1:27017");
        Environment.SetEnvironmentVariable("S3Clients__ClientA__ServiceURL", "http://localhost:4566");
        Environment.SetEnvironmentVariable("S3Clients__ClientA__AccessKey", "test");
        Environment.SetEnvironmentVariable("S3Clients__ClientA__SecretKey", "test");
        Environment.SetEnvironmentVariable("S3Clients__ClientA__Region", "eu-north-1");
        Environment.SetEnvironmentVariable("S3Clients__ClientB__ServiceURL", "http://localhost:4566");
        Environment.SetEnvironmentVariable("S3Clients__ClientB__AccessKey", "test");
        Environment.SetEnvironmentVariable("S3Clients__ClientB__SecretKey", "test");
        Environment.SetEnvironmentVariable("S3Clients__ClientB__Region", "eu-north-1");

        builder.ConfigureServices(services =>
        {
            RemoveService<IHealthCheckPublisher>(services);
        });

        host = base.CreateHost(builder);

        return host;
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var service = services.FirstOrDefault(x => x.ServiceType == typeof(T));
        if (service != null)
        {
            services.Remove(service);
        }
    }
}
