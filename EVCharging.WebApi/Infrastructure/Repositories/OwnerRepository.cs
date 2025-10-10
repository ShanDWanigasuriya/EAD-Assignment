using EVCharging.WebApi.Domain;
using MongoDB.Driver;

namespace EVCharging.WebApi.Infrastructure.Repositories
{
    public class OwnerRepository : BaseRepository<Owner>
    {
        public OwnerRepository(MongoDbService mongo) : base(mongo, "owners")
        {
            // Ensure NIC is unique in the "owners" collection
            var indexKeys = Builders<Owner>.IndexKeys.Ascending(x => x.NIC);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<Owner>(indexKeys, indexOptions);
            Col.Indexes.CreateOne(indexModel);
        }

        public Task<Owner?> FindByNicAsync(string nic) =>
            Col.Find(x => x.NIC == nic).FirstOrDefaultAsync();

        public async Task CreateAsync(Owner o)
        {
            try
            {
                await Col.InsertOneAsync(o);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new InvalidOperationException($"Owner with NIC {o.NIC} already exists.");
            }
        }

        public Task UpdateByNicAsync(string nic, Owner updated)
        {
            var updateDefinition = Builders<Owner>.Update.Combine(
                Builders<Owner>.Update.Set(o => o.FullName, updated.FullName),
                Builders<Owner>.Update.Set(o => o.Email, updated.Email),
                Builders<Owner>.Update.Set(o => o.Phone, updated.Phone)
            );

            return Col.UpdateOneAsync(x => x.NIC == nic, updateDefinition);
        }

        public Task<List<Owner>> GetAllAsync() => Col.Find(_ => true).ToListAsync();

        public Task UpdateActiveByNicAsync(string nic, bool active) =>
            Col.UpdateOneAsync(x => x.NIC == nic, Builders<Owner>.Update.Set(o => o.IsActive, active));

        public Task DeleteByNicAsync(string nic) =>
            Col.DeleteOneAsync(x => x.NIC == nic);
    }
}
