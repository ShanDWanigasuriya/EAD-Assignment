using EVCharging.WebApi.Domain;
using MongoDB.Driver;

namespace EVCharging.WebApi.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(MongoDbService mongo) : base(mongo, "users") { }

        public Task<User?> FindByUsernameAsync(string username) =>
            Col.Find(x => x.Username == username).FirstOrDefaultAsync();

        public Task<User?> FindByIdAsync(string id) =>
            Col.Find(x => x.Id == id).FirstOrDefaultAsync();


        public Task CreateAsync(User user) => Col.InsertOneAsync(user);

        // Update existing user info (username, role, password)
        public Task UpdateUserAsync(string id, User updated)
        {
            var updateDef = Builders<User>.Update.Combine(
                Builders<User>.Update.Set(u => u.Username, updated.Username),
                Builders<User>.Update.Set(u => u.Role, updated.Role),
                Builders<User>.Update.Set(u => u.PasswordHash, updated.PasswordHash)
            );
            return Col.UpdateOneAsync(x => x.Id == id, updateDef);
        }

        public Task<List<User>> GetAllAsync() => Col.Find(_ => true).ToListAsync();

        public Task UpdateActiveAsync(string id, bool active) =>
            Col.UpdateOneAsync(x => x.Id == id, Builders<User>.Update.Set(u => u.IsActive, active));
    }
}
