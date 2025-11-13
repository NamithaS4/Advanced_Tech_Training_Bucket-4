using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace AMIProjectView.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    public class TariffController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TariffController> _logger;

        public TariffController(IHttpClientFactory f, ILogger<TariffController> logger)
        {
            _httpClientFactory = f;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("api");
            try
            {
                var items = await client.GetFromJsonAsync<List<TariffVm>>("api/tariffs");
                return View(items ?? new List<TariffVm>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load tariffs");
                TempData["err"] = "Unable to load tariffs: " + ex.Message;
                return View(new List<TariffVm>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TariffVm vm)
        {
            var client = _httpClientFactory.CreateClient("api");

            try
            {
                var resp = await client.PostAsJsonAsync("api/tariffs", vm);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["ok"] = "Tariff added.";
                }
                else
                {
                    var message = await ReadErrorMessageAsync(resp);
                    TempData["err"] = $"Add failed: {message}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add tariff");
                TempData["err"] = "Failed to add tariff: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Tariff/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("api");
            try
            {
                var vm = await client.GetFromJsonAsync<TariffVm>($"api/tariffs/{id}");
                if (vm == null)
                {
                    TempData["err"] = "Tariff not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load tariff id {Id}", id);
                TempData["err"] = "Failed to load tariff: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Tariff/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TariffVm vm)
        {
            var client = _httpClientFactory.CreateClient("api");

            try
            {
                var resp = await client.PutAsJsonAsync($"api/tariffs/{vm.TariffId}", vm);
                if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    TempData["ok"] = "Tariff updated.";
                }
                else
                {
                    var message = await ReadErrorMessageAsync(resp);
                    TempData["err"] = "Update failed: " + message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update tariff id {Id}", vm.TariffId);
                TempData["err"] = "Update failed: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Try to parse JSON error objects like { error: "..."} or { message: "..." } or fallback to text
        private async Task<string> ReadErrorMessageAsync(HttpResponseMessage resp)
        {
            try
            {
                var raw = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(raw)) return $"Server returned {(int)resp.StatusCode} {resp.ReasonPhrase}";

                // try parse json
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("error", out var perr) && perr.ValueKind == JsonValueKind.String)
                            return perr.GetString() ?? raw;

                        if (root.TryGetProperty("message", out var pmsg) && pmsg.ValueKind == JsonValueKind.String)
                            return pmsg.GetString() ?? raw;
                    }
                }
                catch (JsonException) { /* not JSON - continue */ }

                return raw.Trim('"');
            }
            catch
            {
                return $"Server returned {(int)resp.StatusCode} {resp.ReasonPhrase}";
            }
        }
    }

    public class TariffVm
    {
        public int TariffId { get; set; }
        public string TariffName { get; set; } = "";
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public decimal BaseRate { get; set; }
        public decimal TaxRate { get; set; }
    }
}
