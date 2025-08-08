using Amazon.S3;

namespace Mav.Healthcheck.Infrastructure.Storage.Factories
{
    public interface IS3ClientFactory
    {
        IAmazonS3 GetClient(string name);

        string GetBucketName(string name);

        IEnumerable<string> GetRegisteredClientNames();
    }
}
