using EVCharging.WebApi.Domain;
using MongoDB.Driver;

namespace EVCharging.WebApi.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(MongoDbService mongo) : base(mongo, "users") { }

        public Task<User?> FindByUsernameAsync(string username) =>
            Col.Find(x => x.Username == username).FirstOrDefaultAsync();

        public Task CreateAsync(User user) => Col.InsertOneAsync(user);

        public Task<List<User>> GetAllAsync() => Col.Find(_ => true).ToListAsync();

        public Task UpdateActiveAsync(string id, bool active) =>
            Col.UpdateOneAsync(x => x.Id == id, Builders<User>.Update.Set(u => u.IsActive, active));
    }
}
