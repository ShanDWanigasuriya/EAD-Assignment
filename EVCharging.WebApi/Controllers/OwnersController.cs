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
            try
            {
                await _owners.RegisterAsync(o);
                return Ok(new { message = "Owner registered." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }


        [AllowAnonymous]
        [HttpGet("{nic}")]
        public async Task<IActionResult> GetByNic(string nic)
        {
            var o = await _owners.GetByNicAsync(nic);
            return o is null ? NotFound(new { error = "Owner not found." }) : Ok(o);
        }

        [AllowAnonymous]
        [HttpPut("{nic}")]
        public async Task<IActionResult> Update(string nic, [FromBody] Owner updated)
        {
            try
            {
                await _owners.UpdateAsync(nic, updated);
                return Ok(new { message = "Owner updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPatch("{nic}/deactivate")]
        public async Task<IActionResult> Deactivate(string nic)
        {
            await _owners.DeactivateAsync(nic);
            return Ok(new { message = "Owner deactivated." });
        }

        [Authorize(Roles = "Backoffice")]
        [HttpPatch("{nic}/activate")]
        public async Task<IActionResult> Activate(string nic)
        {
            await _owners.ReactivateAsync(nic);
            return Ok(new { message = "Owner reactivated." });
        }

        [Authorize(Roles = "Backoffice")]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _owners.GetAllAsync());

        [Authorize(Roles = "Backoffice")]
        [HttpDelete("{nic}")]
        public async Task<IActionResult> Delete(string nic)
        {
            await _owners.DeleteAsync(nic);
            return Ok(new { message = "Owner deleted successfully." });
        }

    }
}
