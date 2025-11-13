using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace AMIProjectView.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    public class TariffSlabsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TariffSlabsController> _logger;

        public TariffSlabsController(IHttpClientFactory f, ILogger<TariffSlabsController> logger)
        {
            _httpClientFactory = f;
            _logger = logger;
        }

        // small DTO for requests (keeps content compact)
        private class TariffSlabDto
        {
            public int SlabId { get; set; }
            public int TariffId { get; set; }
            public decimal FromKwh { get; set; }
            public decimal ToKwh { get; set; }
            public decimal RatePerKwh { get; set; }
        }

        // INDEX: show slabs for selected tariff
        [HttpGet]
        public async Task<IActionResult> Index(int? tariffId)
        {
            var client = _httpClientFactory.CreateClient("api");

            var tariffs = await client.GetFromJsonAsync<List<TariffVm>>("api/tariffs") ?? new();
            ViewBag.Tariffs = tariffs;

            var slabs = new List<TariffSlabVm>();
            if (tariffId.HasValue)
            {
                slabs = await client.GetFromJsonAsync<List<TariffSlabVm>>($"api/tariffslabs/{tariffId.Value}") ?? new();
            }

            ViewBag.SelectedTariffId = tariffId;
            return View(slabs);
        }

        // ADD: Accept the VM only — the form must include a hidden TariffId matching model property
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TariffSlabVm vm)
        {
            // vm.TariffId must be provided via hidden input named "TariffId"
            if (vm == null)
            {
                TempData["err"] = "Invalid request.";
                return RedirectToAction(nameof(Index));
            }

            if (vm.TariffId <= 0)
            {
                TempData["err"] = "TariffId is required.";
                return RedirectToAction(nameof(Index), new { tariffId = (int?)null });
            }

            var client = _httpClientFactory.CreateClient("api");

            var dto = new TariffSlabDto
            {
                TariffId = vm.TariffId,
                FromKwh = vm.FromKwh,
                ToKwh = vm.ToKwh,
                RatePerKwh = vm.RatePerKwh
            };

            var resp = await client.PostAsJsonAsync("api/tariffslabs", dto);
            if (resp.IsSuccessStatusCode)
            {
                TempData["ok"] = "Slab added.";
            }
            else
            {
                TempData["err"] = await ReadApiError(resp);
            }

            return RedirectToAction(nameof(Index), new { tariffId = vm.TariffId });
        }

        // EDIT GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("api");
            var resp = await client.GetAsync($"api/tariffslabs/slab/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                TempData["err"] = await ReadApiError(resp);
                return RedirectToAction(nameof(Index));
            }

            var slab = await resp.Content.ReadFromJsonAsync<TariffSlabVm>();
            if (slab == null)
            {
                TempData["err"] = "Slab not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(slab);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TariffSlabVm vm)
        {
            if (vm == null)
            {
                TempData["err"] = "Invalid request.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                // Show model errors in view
                return View(vm);
            }

            var client = _httpClientFactory.CreateClient("api");

            var dto = new TariffSlabDto
            {
                SlabId = vm.SlabId,
                TariffId = vm.TariffId,
                FromKwh = vm.FromKwh,
                ToKwh = vm.ToKwh,
                RatePerKwh = vm.RatePerKwh
            };

            var resp = await client.PutAsJsonAsync($"api/tariffslabs/{vm.SlabId}", dto);
            if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                TempData["ok"] = "Slab updated.";
                return RedirectToAction(nameof(Index), new { tariffId = vm.TariffId });
            }

            // show API error in edit page
            TempData["err"] = await ReadApiError(resp);
            return View(vm);
        }

        // Delete action present server-side for completeness (UI may not expose it)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int slabId, int tariffId)
        {
            var client = _httpClientFactory.CreateClient("api");
            var resp = await client.DeleteAsync($"api/tariffslabs/{slabId}");
            if (resp.IsSuccessStatusCode)
            {
                TempData["ok"] = "Slab deleted.";
            }
            else
            {
                TempData["err"] = await ReadApiError(resp);
            }
            return RedirectToAction(nameof(Index), new { tariffId });
        }

        // helper to convert API response into friendly string
        private async Task<string> ReadApiError(System.Net.Http.HttpResponseMessage resp)
        {
            try
            {
                var raw = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(raw))
                    return $"Server returned {(int)resp.StatusCode} {resp.ReasonPhrase}";

                // try parse JSON object { field, message } or { error } or { message } or ASP.NET validation ProblemDetails
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("field", out var fld) && root.TryGetProperty("message", out var msg))
                        {
                            var f = fld.GetString();
                            var m = msg.GetString();
                            return !string.IsNullOrWhiteSpace(f) ? $"{f}: {m}" : (m ?? raw);
                        }
                        if (root.TryGetProperty("error", out var err) && err.ValueKind == JsonValueKind.String)
                        {
                            return err.GetString() ?? raw;
                        }
                        if (root.TryGetProperty("message", out var m2) && m2.ValueKind == JsonValueKind.String)
                        {
                            return m2.GetString() ?? raw;
                        }

                        // ProblemDetails "errors" object => pick first field error
                        if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                        {
                            foreach (var prop in errors.EnumerateObject())
                            {
                                if (prop.Value.ValueKind == JsonValueKind.Array && prop.Value.GetArrayLength() > 0)
                                {
                                    var first = prop.Value[0].GetString();
                                    if (!string.IsNullOrWhiteSpace(first))
                                        return $"{prop.Name}: {first}";
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // not JSON shaped as expected — fallback to raw text
                }

                return raw.Trim('"');
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReadApiError failed");
                return $"Server returned {(int)resp.StatusCode} {resp.ReasonPhrase}";
            }
        }
    }
}
