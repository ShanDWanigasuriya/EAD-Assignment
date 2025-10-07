using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EVCharging.WebApi.Infrastructure
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _db;
        public MongoDbService(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _db = client.GetDatabase(settings.Value.DatabaseName);
        }
        public IMongoCollection<T> GetCollection<T>(string name) => _db.GetCollection<T>(name);

    }
}
