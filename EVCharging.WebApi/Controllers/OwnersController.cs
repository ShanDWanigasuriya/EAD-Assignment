using EVCharging.WebApi.Domain;
using EVCharging.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVCharging.WebApi.Controllers
{
    [ApiController, Route("api/owners")]
    public class OwnersController : ControllerBase
    {
        private readonly OwnerService _owners;
        public OwnersController(OwnerService owners) => _owners = owners;

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Owner o)
        {
            await _owners.RegisterAsync(o);
            return Ok(new { message = "Owner registered." });
        }

        [AllowAnonymous]
        [HttpGet("{nic}")]
        public async Task<IActionResult> GetByNic(string nic)
        {
            var o = await _owners.GetByNicAsync(nic);
            return o is null ? NotFound() : Ok(o);
        }

        [AllowAnonymous]
        [HttpPut("{nic}")]
        public async Task<IActionResult> Update(string nic, [FromBody] Owner updated)
        {
            await _owners.UpdateAsync(nic, updated);
            return Ok(new { message = "Owner updated." });
        }

        [AllowAnonymous]
        [HttpPatch("{nic}/deactivate")]
        public async Task<IActionResult> Deactivate(string nic) { await _owners.DeactivateAsync(nic); return Ok(); }

        [Authorize(Roles = "Backoffice")]
        [HttpPatch("{nic}/activate")]
        public async Task<IActionResult> Activate(string nic) { await _owners.ReactivateAsync(nic); return Ok(); }

        [Authorize(Roles = "Backoffice")]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _owners.GetAllAsync());
    }
}
