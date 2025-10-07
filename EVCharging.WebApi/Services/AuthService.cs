using EVCharging.WebApi.Infrastructure.Repositories;

namespace EVCharging.WebApi.Services
{
    public class AuthService
    {
        private readonly UserRepository _users;
        public AuthService(UserRepository users) => _users = users;

        public async Task<(bool ok, string role)> LoginAsync(string username, string password)
        {
            var user = await _users.FindByUsernameAsync(username);
            if (user == null || !user.IsActive) return (false, "");

            var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return ok ? (true, user.Role) : (false, "");
        }
    }
}
