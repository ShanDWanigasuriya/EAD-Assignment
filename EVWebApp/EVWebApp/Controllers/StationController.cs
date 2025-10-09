using EVWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EVWebApp.Controllers
{
    public class StationController : Controller
    {
        private readonly ApiClient _api;
        public StationController(ApiClient api) => _api = api;

        // List with computed ActiveBookings + AvailableNow
        public async Task<IActionResult> Index()
        {
            var stations = await _api.Get<JArray>("/api/stations") ?? new JArray();
            var list = new JArray();
            var now = DateTime.UtcNow;

            foreach (JObject s in stations)
            {
                var id = (string?)s["id"] ?? "";

                // Active bookings for this station
                var bookings = await _api.Get<JArray>($"/api/bookings/station/{id}") ?? new JArray();
                var activeBookings = bookings.Count(b =>
                {
                    var st = (string?)b["status"];
                    return st == "Pending" || st == "Approved";
                });
                s["activeBookings"] = activeBookings;

                // Available slots NOW (from availability window) – fallback to total slots
                int totalSlots = (int?)s["slots"] ?? 0;
                int availableNow = totalSlots;

                var win = ((JArray?)s["availability"])?
                    .FirstOrDefault(w =>
                    {
                        var start = DateTime.Parse((string)w["startUtc"]);
                        var end = DateTime.Parse((string)w["endUtc"]);
                        return start <= now && now <= end;
                    }) as JObject;

                if (win != null)
                    availableNow = (int?)win["availableSlots"] ?? totalSlots;

                s["availableNow"] = availableNow;

                // Compute human-readable displayLocation
                var loc = s["location"];
                if (loc != null && loc.HasValues)
                {
                    double lat = (double?)loc["lat"] ?? 0;
                    double lng = (double?)loc["lng"] ?? 0;

                    if (Math.Abs(lat - 6.9271) < 0.001 && Math.Abs(lng - 79.8612) < 0.001)
                        s["displayLocation"] = "Colombo";
                    else if (Math.Abs(lat - 6.0535) < 0.001 && Math.Abs(lng - 80.2210) < 0.001)
                        s["displayLocation"] = "Galle";
                    else if (Math.Abs(lat - 7.2906) < 0.001 && Math.Abs(lng - 80.6337) < 0.001)
                        s["displayLocation"] = "Kandy";
                    else
                        s["displayLocation"] = $"{lat}, {lng}";
                }
                else
                {
                    s["displayLocation"] = "Unknown";
                }

                list.Add(s);
            }

            return View(list);
        }

        // Create (let API generate id)
        [HttpPost]
        public async Task<IActionResult> Add(string name, string location, string type, int slots)
        {
            var body = new
            {
                name,
                type,
                slots,
                isActive = true,
                location = GetCoordinates(location) // accepts {"lat":..., "lng":...}
            };

            var res = await _api.Post("/api/stations", body);
            TempData[res.IsSuccessStatusCode ? "Success" : "Error"] =
                res.IsSuccessStatusCode ? "Station created." : "Failed to create station.";
            return RedirectToAction("Index");
        }

        // Update (PUT /api/stations/{id})
        [HttpPost]
        public async Task<IActionResult> Update(string id, string name, string location, string type, int slots)
        {
            var res = await _api.Put($"/api/stations/{id}", new
            {
                name,
                type,
                slots,
                // keep current active state on server; we send only fields we edit
                location = ParseGeo(location)
            });

            TempData[res.IsSuccessStatusCode ? "Success" : "Error"] =
                res.IsSuccessStatusCode ? "Station updated." : "Failed to update station.";
            return RedirectToAction("Index");
        }

        // Activate / Deactivate
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id, bool isActive)
        {
            var url = isActive
                ? $"/api/stations/{id}/deactivate"
                : $"/api/stations/{id}/activate";

            var res = await _api.Patch(url, new { });
            TempData[res.IsSuccessStatusCode ? "Success" : "Error"] =
                res.IsSuccessStatusCode ? "Status changed."
                                        : "Cannot change status (active bookings may exist).";
            return RedirectToAction("Index");
        }

        // If you really want a red “Delete”, make it a soft delete -> deactivate:
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var res = await _api.Patch($"/api/stations/{id}/deactivate", new { });
            TempData[res.IsSuccessStatusCode ? "Success" : "Error"] =
                res.IsSuccessStatusCode ? "Station deactivated." : "Failed to deactivate station.";
            return RedirectToAction("Index");
        }

        private object ParseGeo(string value)
        {
            try
            {
                var j = JObject.Parse(value);
                return new { lat = (double?)j["lat"] ?? 0d, lng = (double?)j["lng"] ?? 0d };
            }
            catch
            {
                return new { lat = 0d, lng = 0d };
            }
        }
        private object GetCoordinates(string city)
        {
            city = city.ToLowerInvariant();

            return city switch
            {
                "colombo" => new { lat = 6.9271, lng = 79.8612 },
                "galle" => new { lat = 6.0535, lng = 80.2210 },
                "kandy" => new { lat = 7.2906, lng = 80.6337 },
                _ => new { lat = 0.0, lng = 0.0 }
            };
        }
    }
}
