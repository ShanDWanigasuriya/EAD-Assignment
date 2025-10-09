using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using EVWebApp.Services;

namespace EVWebApp.Controllers
{
    public class EvOwnerController : Controller
    {
        private readonly ApiClient _api;
        public EvOwnerController(ApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            // GET /api/owners -> array of Owners
            var arr = await _api.Get<JArray>("/api/owners") ?? new JArray();
            return View(arr);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string nic, string fullName)
        {
            if (string.IsNullOrWhiteSpace(nic) || string.IsNullOrWhiteSpace(fullName))
            {
                TempData["Error"] = "All fields are required.";
                return RedirectToAction("Index");
            }

            // API design uses /api/owners/register for create
            var res = await _api.Post("/api/owners/register", new { nic, fullName });
            TempData[(res.IsSuccessStatusCode ? "Success" : "Error")] =
                res.IsSuccessStatusCode ? "Owner added." : "Failed to add owner.NIC already exists";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string nic, string fullName)
        {
            // PUT /api/owners/{nic}
            var res = await _api.Put($"/api/owners/{nic}", new { fullName });
            TempData[(res.IsSuccessStatusCode ? "Success" : "Error")] =
                res.IsSuccessStatusCode ? "Owner updated." : "Failed to update owner.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string nic)
        {
            // Not shown in swagger, but typical patterns are DELETE /api/owners/{nic}
            // If your teammate didn’t implement DELETE, you can use a “deactivate” to simulate delete.
            var res = await _api.Delete($"/api/owners/{nic}");
            if (!res.IsSuccessStatusCode)
            {
                // fallback: deactivate
                await _api.Patch($"/api/owners/{nic}/deactivate", new { });
            }

            TempData["Success"] = "Owner removed.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(string nic, bool isActive)
        {
            // PATCH /api/owners/{nic}/activate or /deactivate
            var url = isActive
                ? $"/api/owners/{nic}/deactivate"
                : $"/api/owners/{nic}/activate";

            var res = await _api.Patch(url, new { });
            TempData[(res.IsSuccessStatusCode ? "Success" : "Error")] =
                res.IsSuccessStatusCode ? "Status changed." : "Failed to change status.";
            return RedirectToAction("Index");
        }
    }
}

