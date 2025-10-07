using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EVWebApp.Controllers
{
    public class StationController : Controller
    {
        // 🔹 Simulated in-memory data (replace with API later)
        private static JArray stations = JArray.Parse(@"[
            { 'stationId': 'S001', 'name': 'Colombo SuperCharge', 'location': 'Colombo 07', 'type':'AC', 'slots':4, 'isActive': true, 'activeBookings': 2 },
            { 'stationId': 'S002', 'name': 'Galle RapidCharge', 'location': 'Galle', 'type':'DC', 'slots':3, 'isActive': true, 'activeBookings': 0 },
            { 'stationId': 'S003', 'name': 'Kandy GreenCharge', 'location': 'Kandy', 'type':'AC', 'slots':5, 'isActive': false, 'activeBookings': 0 }
        ]");

        public IActionResult Index()
        {
            return View(stations);
        }

        // ✅ CREATE
        [HttpPost]
        public IActionResult Add(string stationId, string name, string location, string type, int slots)
        {
            if (string.IsNullOrWhiteSpace(stationId) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(location) ||
                string.IsNullOrWhiteSpace(type) ||
                slots < 1)
            {
                TempData["Message"] = "⚠️ Please fill all fields correctly.";
                return RedirectToAction("Index");
            }

            // Check duplicate ID
            if (stations.Any(s => s["stationId"]!.ToString() == stationId))
            {
                TempData["Message"] = $"⚠️ Station ID {stationId} already exists!";
                return RedirectToAction("Index");
            }

            var newStation = new JObject
            {
                ["stationId"] = stationId,
                ["name"] = name,
                ["location"] = location,
                ["type"] = type,
                ["slots"] = slots,
                ["isActive"] = true,
                ["activeBookings"] = 0
            };
            stations.Add(newStation);
            TempData["Message"] = $"✅ Station {name} added successfully (simulated)";
            return RedirectToAction("Index");
        }

        // ✅ UPDATE
        [HttpPost]
        public IActionResult Update(string stationId, string name, string location, string type, int slots)
        {
            var station = stations.FirstOrDefault(s => s["stationId"]!.ToString() == stationId);
            if (station == null)
            {
                TempData["Message"] = $"❌ Station {stationId} not found!";
                return RedirectToAction("Index");
            }

            station["name"] = name;
            station["location"] = location;
            station["type"] = type;
            station["slots"] = slots;

            TempData["Message"] = $"✅ Station {stationId} updated successfully (simulated)";
            return RedirectToAction("Index");
        }

        // ✅ DELETE
        [HttpPost]
        public IActionResult Delete(string stationId)
        {
            var station = stations.FirstOrDefault(s => s["stationId"]!.ToString() == stationId);
            if (station != null)
            {
                stations.Remove(station);
                TempData["Message"] = $"🗑️ Station {stationId} deleted successfully (simulated)";
            }
            return RedirectToAction("Index");
        }

        // ✅ ACTIVATE / DEACTIVATE
        [HttpPost]
        public IActionResult ToggleStatus(string stationId, bool isActive, int activeBookings)
        {
            var station = stations.FirstOrDefault(s => s["stationId"]!.ToString() == stationId);
            if (station == null)
            {
                TempData["Message"] = $"❌ Station not found!";
                return RedirectToAction("Index");
            }

            if (activeBookings > 0 && isActive)
            {
                TempData["Message"] = $"❌ Cannot deactivate {stationId} — active bookings exist!";
                return RedirectToAction("Index");
            }

            station["isActive"] = !isActive;
            TempData["Message"] = !isActive
                ? $"✅ Station {stationId} activated successfully (simulated)"
                : $"✅ Station {stationId} deactivated successfully (simulated)";
            return RedirectToAction("Index");
        }
    }
}
