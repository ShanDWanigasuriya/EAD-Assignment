using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EVWebApp.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Simulated authentication
            if (username == "admin" && password == "admin123")
            {
                HttpContext.Session.SetString("username", username);
                HttpContext.Session.SetString("role", "Backoffice");
                return RedirectToAction("Index", "Dashboard");
            }
            else if (username == "station" && password == "station123")
            {
                HttpContext.Session.SetString("username", username);
                HttpContext.Session.SetString("role", "Station Operator");
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
