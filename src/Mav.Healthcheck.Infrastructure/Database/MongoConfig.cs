namespace Mav.Healthcheck.Infrastructure.Database;

public record MongoConfig
{
    public string DatabaseUri { get; init; } = default!;
    public string DatabaseName { get; init; } = default!;
}
