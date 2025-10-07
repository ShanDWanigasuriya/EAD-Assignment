using EVCharging.WebApi.Domain;
using EVCharging.WebApi.Infrastructure.Repositories;

namespace EVCharging.WebApi.Services
{
    public class UserService
    {
        private readonly UserRepository _repo;
        public UserService(UserRepository repo) => _repo = repo;

        public async Task CreateAsync(string username, string password, string role)
        {
            var existing = await _repo.FindByUsernameAsync(username);
            if (existing != null) throw new InvalidOperationException("Username already exists.");

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                IsActive = true
            };
            await _repo.CreateAsync(user);
        }

        public Task<List<User>> GetAllAsync() => _repo.GetAllAsync();

        public Task SetActiveAsync(string id, bool active) => _repo.UpdateActiveAsync(id, active);
    }
}
