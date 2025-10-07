using EVCharging.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EVCharging.WebApi.Controllers
{
    [ApiController, Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _bookings;
        public BookingsController(BookingService bookings) => _bookings = bookings;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest req)
        {
            var id = await _bookings.CreateAsync(
                req.OwnerNic, req.StationId, req.ReservationStartUtc, req.ReservationEndUtc, DateTime.UtcNow);
            return Ok(new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateBookingRequest req)
        {
            await _bookings.UpdateAsync(id, req.NewStartUtc, req.NewEndUtc, DateTime.UtcNow);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            await _bookings.CancelAsync(id, DateTime.UtcNow);
            return Ok();
        }

        [HttpGet("owner/{nic}")]
        public async Task<IActionResult> ByOwner(string nic) => Ok(await _bookings.GetByOwnerAsync(nic));

        [HttpGet("station/{stationId}")]
        public async Task<IActionResult> ByStation(string stationId) => Ok(await _bookings.GetByStationAsync(stationId));

        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> Approve(string id) { await _bookings.ApproveAsync(id); return Ok(); }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> Complete(string id) { await _bookings.CompleteAsync(id); return Ok(); }
    }

    public record CreateBookingRequest(string OwnerNic, string StationId, DateTime ReservationStartUtc, DateTime ReservationEndUtc);
    public record UpdateBookingRequest(DateTime NewStartUtc, DateTime NewEndUtc);
}
