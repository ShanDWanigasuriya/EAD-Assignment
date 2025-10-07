using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EVWebApp.Controllers
{
    public class UserController : Controller
    {
        // Simulated in-memory user list
        private static JArray _users = JArray.Parse(@"[
            { 'id': 1, 'username': 'admin', 'role': 'Backoffice' },
            { 'id': 2, 'username': 'operator1', 'role': 'Station Operator' }
        ]");

        public IActionResult Index()
        {
            // Allow only Backoffice role to access
            var role = HttpContext.Session.GetString("role");
            if (role != "Backoffice")
            {
                TempData["Error"] = "Access denied. Only Backoffice users can manage web users.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View(_users);
        }

        [HttpPost]
        public IActionResult Create(string username, string password, string role)
        {
            int nextId = _users.Count + 1;

            _users.Add(new JObject
            {
                ["id"] = nextId,
                ["username"] = username,
                ["role"] = role
            });

            TempData["Message"] = $"✅ User '{username}' created successfully (simulated)";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(int id, string username, string role)
        {
            var user = _users.FirstOrDefault(u => (int)u["id"] == id);
            if (user != null)
            {
                user["username"] = username;
                user["role"] = role;
                TempData["Message"] = $"✅ User '{username}' updated successfully (simulated)";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _users.FirstOrDefault(u => (int)u["id"] == id);
            if (user != null)
            {
                _users.Remove(user);
                TempData["Message"] = $"🗑️ User deleted successfully (simulated)";
            }
            return RedirectToAction("Index");
        }
    }
}
