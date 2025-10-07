using EVCharging.WebApi.Domain;
using MongoDB.Driver;

namespace EVCharging.WebApi.Infrastructure.Repositories
{
    public class BookingRepository : BaseRepository<Booking>
    {
        public BookingRepository(MongoDbService mongo) : base(mongo, "bookings") { }

        public Task CreateAsync(Booking b) => Col.InsertOneAsync(b);
        public Task<Booking?> GetByIdAsync(string id) =>
            Col.Find(x => x.Id == id).FirstOrDefaultAsync();

        public Task<List<Booking>> GetByOwnerNicAsync(string nic) =>
            Col.Find(x => x.OwnerNic == nic).ToListAsync();

        public Task<List<Booking>> GetByStationAsync(string stationId) =>
            Col.Find(x => x.StationId == stationId).ToListAsync();

        public Task UpdateAsync(Booking b) =>
            Col.ReplaceOneAsync(x => x.Id == b.Id, b);

        public Task UpdateStatusAsync(string id, string status) =>
            Col.UpdateOneAsync(x => x.Id == id, Builders<Booking>.Update.Set(b => b.Status, status));
    }
}
