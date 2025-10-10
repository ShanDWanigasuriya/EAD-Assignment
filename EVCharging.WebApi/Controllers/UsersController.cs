using EVCharging.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVCharging.WebApi.Controllers
{
    [ApiController, Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _users;
        private readonly AuthService _auth;
        public UsersController(UserService users, AuthService auth) { _users = users; _auth = auth; }

        [Authorize(Roles = "Backoffice")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Username and password are required.");

            await _users.CreateAsync(req.Username, req.Password, req.Role ?? "Backoffice");
            return Ok(new { message = "User created." });
        }

        // Update user
        [Authorize(Roles = "Backoffice")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest req)
        {
            await _users.UpdateAsync(id, req.Username, req.Password, req.Role);
            return Ok(new { message = "User updated successfully." });
        }

        [Authorize(Roles = "Backoffice")]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _users.GetAllAsync());

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var token = await _auth.LoginAsync(req.Username, req.Password);

            if (token == null)
                return Unauthorized(new { message = "Invalid credentials." });

            return Ok(new { token });
        }

        [Authorize(Roles = "Backoffice")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(string id) { await _users.SetActiveAsync(id, true); return Ok(); }

        [Authorize(Roles = "Backoffice")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id) { await _users.SetActiveAsync(id, false); return Ok(); }

    }

    public record CreateUserRequest(string Username, string Password, string? Role);
    public record UpdateUserRequest(string? Username, string? Password, string? Role);
    public record LoginRequest(string Username, string Password);
}
