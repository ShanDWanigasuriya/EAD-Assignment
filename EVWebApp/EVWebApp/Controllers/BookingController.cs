using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EVWebApp.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            // 🔹 Simulated data (to be replaced with API integration later)
            var bookings = JArray.Parse(@"[
                { 'bookingId': 'B001', 'stationName': 'Colombo SuperCharge', 'vehicleNo': 'ABC-1234', 'date': '2025-10-08', 'time': '09:00' },
                { 'bookingId': 'B002', 'stationName': 'Galle RapidCharge', 'vehicleNo': 'CAB-4321', 'date': '2025-10-06', 'time': '16:30' }
            ]");

            return View(bookings);
        }

        // 🔹 Create a new booking
        [HttpPost]
        public IActionResult Create(string station, string vehicleNo, DateTime date, TimeSpan time)
        {
            var bookingDateTime = date.Add(time);
            var now = DateTime.Now;

            // Validation rules
            if ((bookingDateTime - now).TotalDays > 7)
            {
                TempData["Message"] = "❌ Booking cannot be made more than 7 days in advance.";
            }
            else if ((bookingDateTime - now).TotalHours < 12)
            {
                TempData["Message"] = "⚠️ Booking must be made at least 12 hours before the selected time.";
            }
            else
            {
                TempData["Message"] = $"✅ Booking created successfully for {station} on {date:yyyy-MM-dd} at {time}. (Simulated)";
            }

            return RedirectToAction("Index");
        }

        // 🔹 Update an existing booking via modal
        [HttpPost]
        public IActionResult Update(string bookingId, string vehicleNo, DateTime date, TimeSpan time)
        {
            var bookingDateTime = date.Add(time);
            var now = DateTime.Now;

            if ((bookingDateTime - now).TotalHours < 12)
            {
                TempData["Message"] = $"⚠️ Cannot update Booking {bookingId}. Updates must be made at least 12 hours before reservation.";
            }
            else
            {
                TempData["Message"] = $"✅ Booking {bookingId} updated successfully (simulated).";
            }

            return RedirectToAction("Index");
        }

        // 🔹 Cancel a booking (must be at least 12 hours before)
        [HttpPost]
        public IActionResult Cancel(string bookingId, DateTime date, TimeSpan time)
        {
            var bookingDateTime = date.Add(time);
            var now = DateTime.Now;

            if ((bookingDateTime - now).TotalHours < 12)
            {
                TempData["Message"] = $"⚠️ Cannot cancel Booking {bookingId}. Cancellations must be done at least 12 hours before reservation.";
            }
            else
            {
                TempData["Message"] = $"🗑️ Booking {bookingId} canceled successfully (simulated).";
            }

            return RedirectToAction("Index");
        }
    }
}
