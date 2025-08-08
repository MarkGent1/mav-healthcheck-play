using Amazon.S3;
using Amazon.S3.Model;
using Mav.Healthcheck.Infrastructure.Storage.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mav.Healthcheck.Infrastructure.Storage.Setup;

public class AwsS3HealthCheck(IAmazonS3 s3Client, StorageConfiguration storageConfiguration) : IHealthCheck
{
    private readonly IAmazonS3 _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
    private readonly StorageConfiguration _storageConfiguration = storageConfiguration ?? throw new ArgumentNullException(nameof(storageConfiguration));

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = _storageConfiguration.DefaultBucketName,
                MaxKeys = 1
            }, cancellationToken);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return HealthCheckResult.Healthy($"S3 bucket '{_storageConfiguration.DefaultBucketName}' is reachable.");
            }

            return HealthCheckResult.Degraded($"S3 bucket '{_storageConfiguration.DefaultBucketName}' returned non-OK status.");
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return HealthCheckResult.Unhealthy($"S3 bucket '{_storageConfiguration.DefaultBucketName}' not found.", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Error accessing S3.", ex);
        }
    }
}
