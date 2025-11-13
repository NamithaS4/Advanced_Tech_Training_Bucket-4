using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace AMIProjectView.Controllers
{
    [Authorize(Policy = "ConsumerPolicy")]
    public class ConsumerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ConsumerController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsumerController(IHttpClientFactory f, ILogger<ConsumerController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = f;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddBearer(HttpClient client)
        {
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                var meters = await client.GetFromJsonAsync<List<MeterViewModel>>("api/meters");
                return View(meters ?? new List<MeterViewModel>());
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error loading meters");
                TempData["err"] = $"Unable to load meters: {ex.Message}";
                return View(new List<MeterViewModel>());
            }
        }


        // ---- ADD THIS ----
        [HttpGet]
        public async Task<IActionResult> Meters(int page = 1, int pageSize = 10)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);
            try
            {
                var all = await client.GetFromJsonAsync<List<MeterViewModel>>("api/meters");
                all = all ?? new List<MeterViewModel>();
                // simple server-side paging in view
                var items = all.OrderBy(m => m.MeterSerialNo).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(all.Count / (double)pageSize));
                return View(items);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error loading meters");
                TempData["err"] = $"Server error: {ex.Message}";
                return View(new List<MeterViewModel>());
            }
        }



        // ---- ADD THIS ----
        [HttpGet]
        public async Task<IActionResult> Bills(string? meterId = null, string? status = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);
            try
            {
                // Build query
                var q = new List<string>();
                if (!string.IsNullOrWhiteSpace(meterId)) q.Add($"meterId={Uri.EscapeDataString(meterId)}");
                if (!string.IsNullOrWhiteSpace(status)) q.Add($"status={Uri.EscapeDataString(status)}");
                if (from.HasValue) q.Add($"from={Uri.EscapeDataString(from.Value.ToString("o"))}");
                if (to.HasValue) q.Add($"to={Uri.EscapeDataString(to.Value.ToString("o"))}");
                q.Add($"page={page}");
                q.Add($"pageSize={pageSize}");
                var url = "api/bills" + (q.Count > 0 ? "?" + string.Join("&", q) : "");

                var resp = await client.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    var raw = await resp.Content.ReadAsStringAsync();
                    TempData["err"] = $"Server error: {(int)resp.StatusCode} {resp.ReasonPhrase}. {raw}";
                    return View(new List<object>()); // bills view model can be implemented
                }

                // API returns { total, page, pageSize, items } when paged
                using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                var root = doc.RootElement;
                List<BillVm> items = new();
                int total = 0;
                if (root.TryGetProperty("items", out var itemsEl) && itemsEl.ValueKind == JsonValueKind.Array)
                {
                    items = JsonSerializer.Deserialize<List<BillVm>>(itemsEl.GetRawText()) ?? new();
                    total = root.GetProperty("total").GetInt32();
                }
                else if (root.ValueKind == JsonValueKind.Array)
                {
                    items = JsonSerializer.Deserialize<List<BillVm>>(root.GetRawText()) ?? new();
                    total = items.Count;
                }

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
                return View(items);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error loading bills");
                TempData["err"] = "Server error: " + ex.Message;
                return View(new List<BillVm>());
            }
        }

        // Consumption placeholder or future call to daily/monthly endpoints
        [HttpGet]
        public IActionResult Consumption()
        {
            // If you have API endpoints for daily/monthly consumption, call them here like above
            return View();
        }
    
        // ---- ADD THIS ----
        [HttpGet]
        public async Task<IActionResult> Monthly(DateTime? from, DateTime? to, string? meterId)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                var q = new System.Collections.Generic.List<string>();
                if (from.HasValue) q.Add($"from={Uri.EscapeDataString(from.Value.ToString("yyyy-MM-dd"))}");
                if (to.HasValue) q.Add($"to={Uri.EscapeDataString(to.Value.ToString("yyyy-MM-dd"))}");
                if (!string.IsNullOrWhiteSpace(meterId)) q.Add($"meterId={Uri.EscapeDataString(meterId)}");
                var qs = q.Count > 0 ? "?" + string.Join("&", q) : "";

                var resp = await client.GetAsync($"api/consumers/me/monthly{qs}");
                if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["err"] = "Not authenticated — please login.";
                    return View(new List<MonthlyConsumptionVm>());
                }
                if (resp.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    TempData["err"] = "Consumer identity missing or invalid. Please re-login.";
                    return View(new List<MonthlyConsumptionVm>());
                }
                resp.EnsureSuccessStatusCode();

                var items = await resp.Content.ReadFromJsonAsync<List<MonthlyConsumptionVm>>() ?? new List<MonthlyConsumptionVm>();
                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading monthly consumption");
                TempData["err"] = "Unable to load consumption: " + ex.Message;
                return View(new List<MonthlyConsumptionVm>());
            }
        }
    }
}
