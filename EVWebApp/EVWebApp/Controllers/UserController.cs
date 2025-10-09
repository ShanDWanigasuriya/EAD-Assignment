using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using EVWebApp.Services;

namespace EVWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApiClient _api;
        public UserController(ApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            // ✅ Only Backoffice users can access
            var role = HttpContext.Session.GetString("role");
            if (role != "Backoffice")
            {
                TempData["Error"] = "Access denied. Only Backoffice users can manage system users.";
                return RedirectToAction("Index", "Dashboard");
            }

            // ✅ Fetch user list from API
            var users = await _api.Get<JArray>("/api/users") ?? new JArray();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                TempData["Error"] = "All fields are required.";
                return RedirectToAction("Index");
            }

            // ✅ POST /api/users/create
            var res = await _api.Post("/api/users/create", new { username, password, role });
            TempData[res.IsSuccessStatusCode ? "Success" : "Error"] =
                res.IsSuccessStatusCode ? "User created successfully." : "Failed to create user.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string username, string role)
        {
            // Your API doesn’t support direct user edit (username/role update)
            // You can later extend Web API for that — for now, just return a message
            TempData["Error"] = "Editing users is not supported via API yet.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            // ✅ Soft delete → deactivate
            var res = await _api.Patch($"/api/users/{id}/deactivate", new { });
            TempData[res.IsSuccessStatusCode ? "Success" : "Error"] =
                res.IsSuccessStatusCode ? "User deactivated successfully." : "Failed to deactivate user.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(string id, bool isActive)
        {
            var url = isActive
                ? $"/api/users/{id}/deactivate"
                : $"/api/users/{id}/activate";

            var res = await _api.Patch(url, new { });
            TempData[res.IsSuccessStatusCode ? "Success" : "Error"] =
                res.IsSuccessStatusCode ? "User status changed." : "Failed to change status.";
            return RedirectToAction("Index");
        }
    }
}
