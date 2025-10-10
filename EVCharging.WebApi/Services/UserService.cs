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
            var allowedRoles = new[] { "Backoffice", "StationOperator" };
            if (!allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Invalid role. Allowed roles: Backoffice or StationOperator.");

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

        // Update user (Backoffice only)
        public async Task UpdateAsync(string id, string? username, string? password, string? role)
        {
            var user = await _repo.FindByIdAsync(id)
                ?? throw new KeyNotFoundException("User not found.");

            if (!string.IsNullOrWhiteSpace(username))
                user.Username = username;

            if (!string.IsNullOrWhiteSpace(password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            if (!string.IsNullOrWhiteSpace(role))
            {
                var allowedRoles = new[] { "Backoffice", "StationOperator" };
                if (!allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Invalid role. Allowed roles: Backoffice or StationOperator.");

                user.Role = role;
            }

            await _repo.UpdateUserAsync(id, user);
        }

        public Task<List<User>> GetAllAsync() => _repo.GetAllAsync();

        public Task SetActiveAsync(string id, bool active) => _repo.UpdateActiveAsync(id, active);

        // Delete user (Backoffice only)
        public async Task DeleteAsync(string id)
        {
            var user = await _repo.FindByIdAsync(id)
                ?? throw new KeyNotFoundException("User not found.");

            await _repo.DeleteUserAsync(id);
        }
    }
}
