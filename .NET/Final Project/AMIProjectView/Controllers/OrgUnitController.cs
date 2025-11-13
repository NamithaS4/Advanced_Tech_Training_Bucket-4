using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AMIProjectView.Models;
using System.Net.Http.Json;

namespace AMIProjectView.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    public class OrgUnitController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OrgUnitController> _logger;

        public OrgUnitController(IHttpClientFactory f, ILogger<OrgUnitController> logger)
        {
            _httpClientFactory = f;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("api");
            var list = await client.GetFromJsonAsync<List<OrgUnitVm>>("api/orgunits");
            return View(list ?? new List<OrgUnitVm>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(OrgUnitVm vm)
        {
            var client = _httpClientFactory.CreateClient("api");

            // Simple sanitization - mirror API normalization
            var payload = new
            {
                Zone = (vm.Zone ?? "").Trim(),
                Substation = (vm.Substation ?? "").Trim(),
                Feeder = (vm.Feeder ?? "").Trim(),
                Dtr = (vm.Dtr ?? "").Trim()
            };

            HttpResponseMessage resp;
            try
            {
                resp = await client.PostAsJsonAsync("api/orgunits", payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API request failed while adding orgunit");
                TempData["err"] = "Unable to call API: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }

            if (resp.IsSuccessStatusCode)
            {
                TempData["ok"] = "Org Unit added.";
                return RedirectToAction(nameof(Index));
            }

            // Read error body and display a friendly message. API returns { error: "..."} or similar.
            string body = await resp.Content.ReadAsStringAsync();
            try
            {
                // try parse json { error: "..." }
                using var doc = System.Text.Json.JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("error", out var p) && p.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    TempData["err"] = p.GetString();
                }
                else
                {
                    TempData["err"] = body;
                }
            }
            catch
            {
                // not JSON
                TempData["err"] = body;
            }

            return RedirectToAction(nameof(Index));
        }
    }

    public class OrgUnitVm
    {
        public int OrgUnitId { get; set; }
        public string? Zone { get; set; }
        public string? Substation { get; set; }
        public string? Feeder { get; set; }
        public string? Dtr { get; set; }
    }
}
