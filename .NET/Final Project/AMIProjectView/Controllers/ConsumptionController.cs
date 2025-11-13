using System.Security.Claims;
using System.Text.Json;
using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace AMIProjectView.Controllers
{
    [Authorize]
    public class ConsumptionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ConsumptionController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsumptionController(IHttpClientFactory httpClientFactory, ILogger<ConsumptionController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddBearer(HttpClient client)
        {
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        private int? TryGetConsumerIdFromClaims()
        {
            var c = User.Claims.FirstOrDefault(x => string.Equals(x.Type, "ConsumerId", StringComparison.OrdinalIgnoreCase));
            if (c != null && int.TryParse(c.Value, out var cid)) return cid;
            var nid = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(nid) && int.TryParse(nid, out var nidInt)) return nidInt;
            var s = _httpContextAccessor.HttpContext?.Session?.GetString("ConsumerId");
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var sid)) return sid;
            return null;
        }

        // GET /Consumption/Daily
        [HttpGet]
        public async Task<IActionResult> Daily(string? meterId, DateTime? from, DateTime? to, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                var isConsumer = User.HasClaim(c => c.Type == "UserType" && c.Value == "Consumer");
                int? consumerId = null;
                if (isConsumer) consumerId = TryGetConsumerIdFromClaims();

                // Build api url
                var url = "api/consumption/daily?";
                if (consumerId.HasValue) url += $"consumerId={consumerId.Value}&";
                if (!string.IsNullOrWhiteSpace(meterId)) url += $"meterId={Uri.EscapeDataString(meterId)}&";
                if (from.HasValue) url += $"from={from.Value:yyyy-MM-dd}&";
                if (to.HasValue) url += $"to={to.Value:yyyy-MM-dd}&";
                url += $"page={page}&pageSize={pageSize}";

                var resp = await client.GetAsync(url);
                List<DailyConsumptionVm> items = new();
                int total = 0;
                if (resp.IsSuccessStatusCode)
                {
                    // try to parse paged wrapper
                    try
                    {
                        var root = await resp.Content.ReadFromJsonAsync<JsonElement?>();
                        if (root.HasValue && root.Value.ValueKind == JsonValueKind.Object && root.Value.TryGetProperty("items", out var itemsEl))
                        {
                            items = JsonSerializer.Deserialize<List<DailyConsumptionVm>>(itemsEl.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                            total = root.Value.TryGetProperty("total", out var totEl) && totEl.TryGetInt32(out var tot) ? tot : items.Count;
                        }
                        else
                        {
                            // not paged -> array
                            items = await resp.Content.ReadFromJsonAsync<List<DailyConsumptionVm>>() ?? new();
                            total = items.Count;
                        }
                    }
                    catch
                    {
                        items = await resp.Content.ReadFromJsonAsync<List<DailyConsumptionVm>>() ?? new();
                        total = items.Count;
                    }
                }
                else
                {
                    // fallback to show message and empty set
                    TempData["err"] = $"Unable to load daily consumption: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                }

                ViewBag.Total = total;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Daily failed");
                TempData["err"] = "Unable to load daily consumption: " + ex.Message;
                return View(new List<DailyConsumptionVm>());
            }
        }

        // GET /Consumption/Monthly
        [HttpGet]
        public async Task<IActionResult> Monthly(string? meterId, DateTime? from, DateTime? to, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                var isConsumer = User.HasClaim(c => c.Type == "UserType" && c.Value == "Consumer");
                int? consumerId = null;
                if (isConsumer) consumerId = TryGetConsumerIdFromClaims();

                var url = "api/consumption/monthly?";
                if (consumerId.HasValue) url += $"consumerId={consumerId.Value}&";
                if (!string.IsNullOrWhiteSpace(meterId)) url += $"meterId={Uri.EscapeDataString(meterId)}&";
                if (from.HasValue) url += $"from={from.Value:yyyy-MM-dd}&";
                if (to.HasValue) url += $"to={to.Value:yyyy-MM-dd}&";
                url += $"page={page}&pageSize={pageSize}";

                var resp = await client.GetAsync(url);
                List<MonthlyConsumptionVm> items = new();
                int total = 0;
                if (resp.IsSuccessStatusCode)
                {
                    try
                    {
                        var root = await resp.Content.ReadFromJsonAsync<JsonElement?>();
                        if (root.HasValue && root.Value.ValueKind == JsonValueKind.Object && root.Value.TryGetProperty("items", out var itemsEl))
                        {
                            items = JsonSerializer.Deserialize<List<MonthlyConsumptionVm>>(itemsEl.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                            total = root.Value.TryGetProperty("total", out var totEl) && totEl.TryGetInt32(out var tot) ? tot : items.Count;
                        }
                        else
                        {
                            items = await resp.Content.ReadFromJsonAsync<List<MonthlyConsumptionVm>>() ?? new();
                            total = items.Count;
                        }
                    }
                    catch
                    {
                        items = await resp.Content.ReadFromJsonAsync<List<MonthlyConsumptionVm>>() ?? new();
                        total = items.Count;
                    }
                }
                else
                {
                    TempData["err"] = $"Unable to load monthly consumption: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                }

                ViewBag.Total = total;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Monthly failed");
                TempData["err"] = "Unable to load monthly consumption: " + ex.Message;
                return View(new List<MonthlyConsumptionVm>());
            }
        }
    }
}
