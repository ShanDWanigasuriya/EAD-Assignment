using EVCharging.WebApi.Domain;
using MongoDB.Driver;

namespace EVCharging.WebApi.Infrastructure.Repositories
{
    public class StationRepository : BaseRepository<Station>
    {
        public StationRepository(MongoDbService mongo) : base(mongo, "stations") { }

        public Task CreateAsync(Station s) => Col.InsertOneAsync(s);
        public Task<List<Station>> GetAllAsync() => Col.Find(_ => true).ToListAsync();
        public Task<Station?> FindByIdAsync(string id) =>
        Col.Find(x => x.Id == id).FirstOrDefaultAsync();

        public Task UpdateAsync(Station s) =>
            Col.ReplaceOneAsync(x => x.Id == s.Id, s);

        public Task UpdateActiveAsync(string id, bool active) =>
            Col.UpdateOneAsync(x => x.Id == id, Builders<Station>.Update.Set(s => s.IsActive, active));
    }
}
