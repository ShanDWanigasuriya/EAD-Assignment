using MongoDB.Driver;

namespace EVCharging.WebApi.Infrastructure.Repositories
{
    public abstract class BaseRepository<T>
    {
        protected readonly IMongoCollection<T> Col;
        protected BaseRepository(MongoDbService mongo, string collectionName)
            => Col = mongo.GetCollection<T>(collectionName);
    }
}
