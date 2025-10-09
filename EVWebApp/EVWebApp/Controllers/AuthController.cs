using Microsoft.AspNetCore.Mvc;
using EVWebApp.Services;
using Newtonsoft.Json.Linq;

namespace EVWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiClient _api;
        public AuthController(ApiClient api) => _api = api;

        [HttpGet]
        public IActionResult Login() => View("~/Views/Auth/Login.cshtml");

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // API login (no bearer for this one)
            var res = await _api.Post("/api/users/login", new { username, password }, auth: false);
            if (!res.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid username / password.";
                return View("~/Views/Auth/Login.cshtml");
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync());
            // Expecting { token: "...", role: "BackOffice" }  (your teammate’s API returns this)
            var token = json.Value<string>("token");
            var role = json.Value<string>("role")
                        ?? json.SelectToken("user.role")?.ToString()
                        ?? "Backoffice";

            // Normalize role text to match the UI you already used
            role = role.Equals("BackOffice", StringComparison.OrdinalIgnoreCase) ? "Backoffice" : role;

            HttpContext.Session.SetString("token", token ?? "");
            HttpContext.Session.SetString("username", username);
            HttpContext.Session.SetString("role", role);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
