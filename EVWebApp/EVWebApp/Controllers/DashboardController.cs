using Microsoft.AspNetCore.Mvc;

namespace EVWebApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // later we’ll call API to load stats
            ViewBag.TotalOwners = 56;
            ViewBag.TotalStations = 12;
            ViewBag.TotalBookings = 143;
            return View();
        }
    }
}
