namespace Mav.Healthcheck.Infrastructure.Storage.Configuration;

public record StorageConfiguration
{
    public required string DefaultBucketName { get; set; }
}
