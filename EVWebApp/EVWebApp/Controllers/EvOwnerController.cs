using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace EVWebApp.Controllers
{
    public class EvOwnerController : Controller
    {
        private static List<JObject> _owners = new()
        {
            new JObject { ["nic"] = "982545678V", ["fullName"] = "Kamal Perera", ["isActive"] = true },
            new JObject { ["nic"] = "200045678V", ["fullName"] = "Nimali Fernando", ["isActive"] = false }
        };

        public IActionResult Index()
        {
            return View(JArray.FromObject(_owners));
        }

        [HttpPost]
        public IActionResult Add(string nic, string fullName)
        {
            if (string.IsNullOrWhiteSpace(nic) || string.IsNullOrWhiteSpace(fullName))
                TempData["Error"] = "All fields are required.";
            else if (_owners.Any(o => o["nic"]?.ToString() == nic))
                TempData["Error"] = "NIC already exists.";
            else
            {
                _owners.Add(new JObject { ["nic"] = nic, ["fullName"] = fullName, ["isActive"] = true });
                TempData["Success"] = "Owner added successfully.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(string nic, string fullName)
        {
            var owner = _owners.FirstOrDefault(o => o["nic"]?.ToString() == nic);
            if (owner != null)
            {
                owner["fullName"] = fullName;
                TempData["Success"] = "Owner updated successfully.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string nic)
        {
            var owner = _owners.FirstOrDefault(o => o["nic"]?.ToString() == nic);
            if (owner != null)
            {
                _owners.Remove(owner);
                TempData["Success"] = "Owner deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleActive(string nic, bool isActive)
        {
            var owner = _owners.FirstOrDefault(o => o["nic"]?.ToString() == nic);
            if (owner != null)
                owner["isActive"] = !isActive;

            TempData["Success"] = $"Owner {(isActive ? "deactivated" : "activated")} successfully (simulated).";
            return RedirectToAction("Index");
        }
    }
}
