using EVCharging.WebApi.Domain;
using MongoDB.Driver;

namespace EVCharging.WebApi.Infrastructure.Repositories
{
    public class OwnerRepository : BaseRepository<Owner>
    {
        public OwnerRepository(MongoDbService mongo) : base(mongo, "owners") { }

        public Task<Owner?> FindByNicAsync(string nic) =>
            Col.Find(x => x.NIC == nic).FirstOrDefaultAsync();

        public Task CreateAsync(Owner o) => Col.InsertOneAsync(o);
        public Task ReplaceAsync(string id, Owner o) =>
            Col.ReplaceOneAsync(x => x.Id == id, o);
        public Task<List<Owner>> GetAllAsync() => Col.Find(_ => true).ToListAsync();

        public Task UpdateActiveByNicAsync(string nic, bool active) =>
            Col.UpdateOneAsync(x => x.NIC == nic, Builders<Owner>.Update.Set(o => o.IsActive, active));
    }
}
