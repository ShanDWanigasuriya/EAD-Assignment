using EVCharging.WebApi.Domain;
using EVCharging.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EVCharging.WebApi.Controllers
{
    [ApiController, Route("api/stations")]
    public class StationsController : ControllerBase
    {
        private readonly StationService _stations;
        public StationsController(StationService stations) => _stations = stations;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Station s) { await _stations.CreateAsync(s); return Ok(s); }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _stations.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var s = await _stations.GetByIdAsync(id);
            return s is null ? NotFound() : Ok(s);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Station s)
        {
            s.Id = id;
            await _stations.UpdateAsync(s);
            return Ok(s);
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(string id) { await _stations.SetActiveAsync(id, true); return Ok(); }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(string id)
        {
            await _stations.SetActiveAsync(id, false);  // will throw if active bookings exist
            return Ok();
        }
    }
}
