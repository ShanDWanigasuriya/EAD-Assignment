using EVCharging.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EVCharging.WebApi.Controllers
{
    [ApiController, Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _users;
        private readonly AuthService _auth;
        public UsersController(UserService users, AuthService auth) { _users = users; _auth = auth; }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username and password are required.");

            await _users.CreateAsync(req.Username, req.Password, req.Role ?? "Backoffice");
            return Ok(new { message = "User created." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _users.GetAllAsync());

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var (ok, role) = await _auth.LoginAsync(req.Username, req.Password);
            if (!ok) return Unauthorized(new { message = "Invalid credentials." });
            // return role + basic identity (JWT can be added later if needed)
            return Ok(new { role });
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(string id) { await _users.SetActiveAsync(id, true); return Ok(); }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id) { await _users.SetActiveAsync(id, false); return Ok(); }
    }

    public record CreateUserRequest(string Username, string Password, string? Role);
    public record LoginRequest(string Username, string Password);
}
