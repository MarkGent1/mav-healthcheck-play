using Amazon.S3;
using Microsoft.Extensions.Configuration;

namespace Mav.Healthcheck.Infrastructure.Storage.Factories.Implementations;

public class S3ClientFactory : IS3ClientFactory
{
    private readonly Dictionary<string, (IAmazonS3 Client, string BucketName)> _clients;

    public S3ClientFactory(IConfiguration config)
    {
        _clients = [];

        var sections = config.GetSection("S3Clients").GetChildren();
        foreach (var section in sections)
        {
            var name = section.Key;
            var accessKey = section["AccessKey"];
            var secretKey = section["SecretKey"];
            var region = section["Region"];
            var serviceUrl = section["ServiceURL"];
            var bucketName = section["BucketName"];

            var s3Config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                AuthenticationRegion = region,
                ForcePathStyle = true,
                UseHttp = true
            };

            var client = new AmazonS3Client(accessKey, secretKey, s3Config);
            _clients[name] = (client!, bucketName!);
        }
    }

    // public IAmazonS3 GetClient(string name) => _clients[name].Client;
    public IAmazonS3 GetClient(string name)
    {
        if (!_clients.TryGetValue(name, out var client))
            throw new KeyNotFoundException($"No S3 client registered for name '{name}'");

        return client.Client;
    }

    // public string GetBucketName(string name) => _clients[name].BucketName;
    public string GetBucketName(string name)
    {
        if (!_clients.TryGetValue(name, out var client))
            throw new KeyNotFoundException($"No S3 client registered for name '{name}'");

        return client.BucketName;
    }

    public IEnumerable<string> GetRegisteredClientNames() => _clients.Keys;
}
