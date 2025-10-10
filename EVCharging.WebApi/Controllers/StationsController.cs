using EVCharging.WebApi.Domain;
using EVCharging.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVCharging.WebApi.Controllers
{
    [ApiController, Route("api/stations")]
    public class StationsController : ControllerBase
    {
        private readonly StationService _stations;
        public StationsController(StationService stations) => _stations = stations;

        [Authorize(Roles = "Backoffice")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Station s) { await _stations.CreateAsync(s); return Ok(s); }

        [Authorize(Roles = "Backoffice,StationOperator")]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _stations.GetAllAsync());

        [Authorize(Roles = "Backoffice,StationOperator")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var s = await _stations.GetByIdAsync(id);
            return s is null ? NotFound() : Ok(s);
        }

        [Authorize(Roles = "Backoffice")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Station s)
        {
            try
            {
                await _stations.UpdateAsync(id, s);
                return Ok(new { message = "Station updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Backoffice")]
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(string id) { await _stations.SetActiveAsync(id, true); return Ok(); }

        [Authorize(Roles = "Backoffice")]
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id)
        {
            await _stations.SetActiveAsync(id, false);  // will throw if active bookings exist
            return Ok();
        }
    }
}
