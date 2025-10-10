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

        public Task<List<StationSlotWindow>> GetAvailabilityAsync(string stationId, DateTime fromUtc, DateTime toUtc) =>
            Col.Find(s => s.Id == stationId)
               .Project(s => s.Availability
                   .Where(a => a.StartUtc >= fromUtc && a.EndUtc <= toUtc && a.AvailableSlots > 0)
                   .ToList())
               .FirstOrDefaultAsync();

        public async Task<bool> TryReserveSlotAsync(string stationId, DateTime startUtc, DateTime endUtc)
        {
            var filter = Builders<Station>.Filter.And(
                Builders<Station>.Filter.Eq(s => s.Id, stationId),
                Builders<Station>.Filter.ElemMatch(
                    s => s.Availability,
                    a => a.StartUtc <= startUtc && a.EndUtc >= endUtc && a.AvailableSlots > 0
                )
            );

            var update = Builders<Station>.Update.Inc("Availability.$.AvailableSlots", -1);

            var updated = await Col.FindOneAndUpdateAsync(
                filter, update,
                new FindOneAndUpdateOptions<Station, Station> { ReturnDocument = ReturnDocument.After });

            return updated != null; // null => no matching window or no capacity
        }

        public async Task<bool> ReleaseSlotAsync(string stationId, DateTime startUtc, DateTime endUtc)
        {
            var filter = Builders<Station>.Filter.And(
                Builders<Station>.Filter.Eq(s => s.Id, stationId),
                Builders<Station>.Filter.ElemMatch(
                    s => s.Availability,
                    a => a.StartUtc <= startUtc && a.EndUtc >= endUtc
                )
            );

            var update = Builders<Station>.Update.Inc("Availability.$.AvailableSlots", +1);

            var updated = await Col.FindOneAndUpdateAsync(
                filter, update,
                new FindOneAndUpdateOptions<Station, Station> { ReturnDocument = ReturnDocument.After });

            return updated != null;
        }

    }
}
