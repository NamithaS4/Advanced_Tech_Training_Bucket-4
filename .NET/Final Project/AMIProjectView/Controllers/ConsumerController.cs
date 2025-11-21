using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Security.Claims;

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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            // Prepare default viewbag values
            ViewBag.PendingBillsCount = 0;
            ViewBag.PendingBills = new List<BillVm>();
            ViewBag.ThisMonthKwh = 0m;
            ViewBag.RecentConsumption = new List<MonthlyConsumptionVm>();

            try
            {
                // 1) Load meters as before
                var meters = await client.GetFromJsonAsync<List<MeterViewModel>>("api/meters") ?? new List<MeterViewModel>();

                // 2) Load pending and half-paid bills for this consumer via consumer-scoped endpoint
                try
                {
                    // Get all bills first
                    var allBillsResp = await client.GetAsync("api/consumers/me/bills");
                    if (allBillsResp.IsSuccessStatusCode)
                    {
                        // parse as array
                        var allBills = await allBillsResp.Content.ReadFromJsonAsync<List<BillVm>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BillVm>();
                        
                        // Filter for Pending and HalfPaid bills
                        var pendingAndHalfPaidBills = allBills.Where(b => 
                            string.Equals(b.Status, "Pending", StringComparison.OrdinalIgnoreCase) || 
                            string.Equals(b.Status, "HalfPaid", StringComparison.OrdinalIgnoreCase)).ToList();
                        
                        ViewBag.PendingBillsCount = pendingAndHalfPaidBills.Count;
                        // keep recent (most recent first)
                        ViewBag.PendingBills = pendingAndHalfPaidBills.OrderByDescending(b => b.GeneratedAt).Take(5).ToList();
                    }
                    else
                    {
                        // if unauthorized/forbidden show 0 and a message could be set in TempData
                        _logger.LogWarning("Unable to fetch bills: {Status} {Reason}", (int)allBillsResp.StatusCode, allBillsResp.ReasonPhrase);
                    }
                }
                catch (Exception exBills)
                {
                    _logger.LogError(exBills, "Error loading pending bills for dashboard");
                }

                // 3) Load this month's monthly consumption for this consumer
                try
                {
                    var now = DateTime.UtcNow;
                    var firstOfMonth = new DateTime(now.Year, now.Month, 1);
                    // last day of month (inclusive)
                    var lastOfMonth = firstOfMonth.AddMonths(1).AddDays(-1);

                    var q = $"from={firstOfMonth:yyyy-MM-dd}&to={lastOfMonth:yyyy-MM-dd}";
                    var consResp = await client.GetAsync($"api/consumers/me/monthly?{q}");
                    if (consResp.IsSuccessStatusCode)
                    {
                        var monthly = await consResp.Content.ReadFromJsonAsync<List<MonthlyConsumptionVm>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MonthlyConsumptionVm>();
                        ViewBag.RecentConsumption = monthly.OrderByDescending(m => m.MonthStartDate).Take(5).ToList();
                        ViewBag.ThisMonthKwh = monthly.Sum(m => m.ConsumptionkWh);
                    }
                    else
                    {
                        _logger.LogWarning("Unable to fetch monthly consumption: {Status} {Reason}", (int)consResp.StatusCode, consResp.ReasonPhrase);
                    }
                }
                catch (Exception exCons)
                {
                    _logger.LogError(exCons, "Error loading monthly consumption for dashboard");
                }

                return View(meters);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error loading meters");
                TempData["err"] = $"Unable to load meters: {ex.Message}";
                // return empty list to view
                return View(new List<MeterViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Bills(string? status, DateTime? from, DateTime? to, string? meterId, int page = 1, int pageSize = 10)
        {
            return RedirectToAction("Index", "Bills", new
            {
                status,
                from = from?.ToString("yyyy-MM-dd"),
                to = to?.ToString("yyyy-MM-dd"),
                meterId,
                page,
                pageSize
            });
        }

        [HttpGet]
        public async Task<IActionResult> Meters(int page = 1, int pageSize = 10)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);
            try
            {
                var all = await client.GetFromJsonAsync<List<MeterViewModel>>("api/meters");
                all = all ?? new List<MeterViewModel>();
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

        [HttpGet]
        public IActionResult Consumption()
        {
            var cid = TryGetConsumerIdFromClaims();
            ViewBag.ConsumerId = cid?.ToString() ?? "";
            return View();
        }

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

        // ---------------------------
        // Proxy endpoints for client JS
        // ---------------------------

        /// <summary>
        /// DAILY proxy — corrected to call the API ConsumptionController (api/consumption/daily).
        /// This mirrors how monthly works (which uses api/consumers/me/monthly).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DailyJson(string? meterId, DateTime? from, DateTime? to)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                var q = new List<string>();
                if (!string.IsNullOrWhiteSpace(meterId)) q.Add($"meterId={Uri.EscapeDataString(meterId)}");
                if (from.HasValue) q.Add($"from={Uri.EscapeDataString(from.Value.ToString("yyyy-MM-dd"))}");
                if (to.HasValue) q.Add($"to={Uri.EscapeDataString(to.Value.ToString("yyyy-MM-dd"))}");
                var qs = q.Count > 0 ? "?" + string.Join("&", q) : "";

                // <<-- CORRECTED: call api/consumption/daily (not api/consumers/me/daily)
                var resp = await client.GetAsync($"api/consumption/daily{qs}");
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return StatusCode((int)resp.StatusCode, body);
                }

                // Try to parse as array of rows. The API may return either an array or a paged wrapper;
                // If the API returned paged { total, page, pageSize, items }, attempt to extract items.
                try
                {
                    var jsonDoc = JsonDocument.Parse(body);
                    if (jsonDoc.RootElement.ValueKind == JsonValueKind.Object && jsonDoc.RootElement.TryGetProperty("items", out var itemsEl))
                    {
                        var items = JsonSerializer.Deserialize<List<DailyConsumptionVm>>(itemsEl.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<DailyConsumptionVm>();
                        return Json(items);
                    }
                }
                catch
                {
                    // ignore and try array parse below
                }

                var arr = JsonSerializer.Deserialize<List<DailyConsumptionVm>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<DailyConsumptionVm>();
                return Json(arr);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying daily consumption");
                return StatusCode(500, "Server error while fetching consumption.");
            }
        }

        /// <summary>
        /// MONTHLY proxy — calls api/consumers/me/monthly (kept as before).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MonthlyJson(string? meterId, DateTime? from, DateTime? to)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                var q = new List<string>();
                if (!string.IsNullOrWhiteSpace(meterId)) q.Add($"meterId={Uri.EscapeDataString(meterId)}");
                if (from.HasValue) q.Add($"from={Uri.EscapeDataString(from.Value.ToString("yyyy-MM-dd"))}");
                if (to.HasValue) q.Add($"to={Uri.EscapeDataString(to.Value.ToString("yyyy-MM-dd"))}");
                var qs = q.Count > 0 ? "?" + string.Join("&", q) : "";

                var resp = await client.GetAsync($"api/consumers/me/monthly{qs}");
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    return StatusCode((int)resp.StatusCode, body);
                }

                // similar handling for paged or array
                try
                {
                    var jsonDoc = JsonDocument.Parse(body);
                    if (jsonDoc.RootElement.ValueKind == JsonValueKind.Object && jsonDoc.RootElement.TryGetProperty("items", out var itemsEl))
                    {
                        var items = JsonSerializer.Deserialize<List<MonthlyConsumptionVm>>(itemsEl.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MonthlyConsumptionVm>();
                        return Json(items);
                    }
                }
                catch
                {
                }

                var arr = JsonSerializer.Deserialize<List<MonthlyConsumptionVm>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MonthlyConsumptionVm>();
                return Json(arr);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying monthly consumption");
                return StatusCode(500, "Server error while fetching consumption.");
            }
        }

        // ---------------------------
        // Meter details page (keeps the same simple behavior)
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> MeterDetails(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
            {
                TempData["err"] = "Meter serial required.";
                return RedirectToAction(nameof(Meters));
            }

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                var resp = await client.GetAsync($"api/meters/{Uri.EscapeDataString(serial)}");
                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    resp = await client.GetAsync($"api/meters?serial={Uri.EscapeDataString(serial)}");
                }

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    TempData["err"] = $"Unable to load meter: {(int)resp.StatusCode} {resp.ReasonPhrase}. {body}";
                    return RedirectToAction(nameof(Meters));
                }

                var meter = await resp.Content.ReadFromJsonAsync<MeterViewModel>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (meter == null)
                {
                    TempData["err"] = "Unable to parse meter response.";
                    return RedirectToAction(nameof(Meters));
                }

                return View(meter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed loading meter details for {Serial}", serial);
                TempData["err"] = "Unable to load meter details: " + ex.Message;
                return RedirectToAction(nameof(Meters));
            }
        }
    }
}
