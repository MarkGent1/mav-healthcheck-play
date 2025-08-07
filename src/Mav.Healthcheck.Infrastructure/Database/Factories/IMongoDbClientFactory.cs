using MongoDB.Driver;

namespace Mav.Healthcheck.Infrastructure.Database.Factories;

public interface IMongoDbClientFactory
{
    IMongoClient GetClient();

    IMongoCollection<T> GetCollection<T>(string collection);
}
