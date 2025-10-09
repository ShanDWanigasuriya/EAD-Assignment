using EVWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EVWebApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApiClient _api;
        public BookingController(ApiClient api) => _api = api;

        // 🔹 GET /Booking
        public async Task<IActionResult> Index()
        {
            var stations = await _api.Get<JArray>("/api/stations") ?? new JArray();
            var owners = await _api.Get<JArray>("/api/owners") ?? new JArray();
            var bookings = new JArray();

            // Fetch all bookings by iterating over owners
            foreach (var o in owners)
            {
                var nic = (string?)o["nic"];
                if (!string.IsNullOrEmpty(nic))
                {
                    var list = await _api.Get<JArray>($"/api/bookings/owner/{nic}");
                    if (list != null)
                        foreach (var b in list)
                            bookings.Add(b);
                }
            }

            // Attach readable station names
            foreach (JObject b in bookings)
            {
                var sid = (string?)b["stationId"];
                var station = stations.FirstOrDefault(s => (string?)s["id"] == sid);
                b["stationName"] = station?["name"] ?? "Unknown Station";
            }

            ViewBag.Stations = stations;
            return View(bookings);
        }

        // 🔹 POST /Booking/Create
        [HttpPost]
        public async Task<IActionResult> Create(string station, string ownerNic, DateTime date, TimeSpan time)
        {
            var startUtc = date.Add(time).ToUniversalTime();
            var endUtc = startUtc.AddHours(1);

            var body = new
            {
                ownerNic,
                stationId = station,
                reservationStartUtc = startUtc,
                reservationEndUtc = endUtc
            };

            var res = await _api.Post("/api/bookings", body);
            TempData["Message"] = res.IsSuccessStatusCode
                ? "✅ Booking created successfully."
                : "❌ Failed to create booking.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string bookingId, DateTime date, TimeSpan time)
        {
            var newStartUtc = date.Add(time).ToUniversalTime();
            var newEndUtc = newStartUtc.AddHours(1);

            var res = await _api.Put($"/api/bookings/{bookingId}", new
            {
                newStartUtc,
                newEndUtc
            });

            TempData["Message"] = res.IsSuccessStatusCode
                ? "✅ Booking updated successfully."
                : "⚠️ Failed to update booking.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string bookingId)
        {
            var res = await _api.Delete($"/api/bookings/{bookingId}");
            TempData["Message"] = res.IsSuccessStatusCode
                ? "🗑️ Booking canceled."
                : "⚠️ Failed to cancel booking.";
            return RedirectToAction("Index");
        }
    }
}
