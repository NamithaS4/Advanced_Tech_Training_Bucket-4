using System.Security.Claims;
using System.Text.Json;
using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace AMIProjectView.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BillsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BillsController(IHttpClientFactory httpClientFactory, ILogger<BillsController> logger, IHttpContextAccessor httpContextAccessor)
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

        // GET /Bills?status=&from=&to=&meterId=&page=&pageSize=
        [HttpGet]
        public async Task<IActionResult> Index(string? status, DateTime? from, DateTime? to, string? meterId, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
                int? consumerId = null;
                if (isConsumer) consumerId = TryGetConsumerIdFromClaims();

                // Try API-side filtering/paging first (if consumerId present we request consumerId)
                var url = $"api/bills?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrWhiteSpace(status)) url += $"&status={Uri.EscapeDataString(status)}";
                if (!string.IsNullOrWhiteSpace(meterId)) url += $"&meterId={Uri.EscapeDataString(meterId)}";
                if (from.HasValue) url += $"&from={from.Value:yyyy-MM-dd}";
                if (to.HasValue) url += $"&to={to.Value:yyyy-MM-dd}";
                if (consumerId.HasValue) url += $"&consumerId={consumerId.Value}";

                var resp = await client.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    // Try parse paged wrapper
                    try
                    {
                        using var stream = await resp.Content.ReadAsStreamAsync();
                        var wrapper = await JsonSerializer.DeserializeAsync<BillsPagedResponse?>(stream, jsonOptions);
                        if (wrapper != null && wrapper.Items != null)
                        {
                            ViewBag.Total = wrapper.Total;
                            ViewBag.Page = wrapper.Page;
                            ViewBag.PageSize = wrapper.PageSize;
                            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(wrapper.Total / (double)wrapper.PageSize));

                            // forward filters for UI
                            ViewBag.StatusFilter = status ?? "";
                            ViewBag.MeterFilter = meterId ?? "";
                            ViewBag.FromFilter = from?.ToString("yyyy-MM-dd") ?? "";
                            ViewBag.ToFilter = to?.ToString("yyyy-MM-dd") ?? "";

                            return View(wrapper.Items);
                        }
                    }
                    catch
                    {
                        // ignore and try array fallback
                    }

                    // Array fallback (non-paged API)
                    try
                    {
                        var arr = await resp.Content.ReadFromJsonAsync<List<BillVm>>(jsonOptions);
                        if (arr != null)
                        {
                            // apply server filters again on client side if necessary
                            if (!string.IsNullOrWhiteSpace(status))
                                arr = arr.Where(b => string.Equals(b.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();

                            if (!string.IsNullOrWhiteSpace(meterId))
                                arr = arr.Where(b => string.Equals(b.MeterID, meterId, StringComparison.OrdinalIgnoreCase)).ToList();

                            if (from.HasValue)
                                arr = arr.Where(b => b.MonthStartDate >= from.Value.Date).ToList();

                            if (to.HasValue)
                                arr = arr.Where(b => b.MonthStartDate <= to.Value.Date).ToList();

                            arr = arr.OrderByDescending(b => b.MonthStartDate).ToList();
                            var total = arr.Count;
                            var items = arr.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                            ViewBag.Total = total;
                            ViewBag.Page = page;
                            ViewBag.PageSize = pageSize;
                            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

                            ViewBag.StatusFilter = status ?? "";
                            ViewBag.MeterFilter = meterId ?? "";
                            ViewBag.FromFilter = from?.ToString("yyyy-MM-dd") ?? "";
                            ViewBag.ToFilter = to?.ToString("yyyy-MM-dd") ?? "";

                            return View(items);
                        }
                    }
                    catch
                    {
                        // final fallback
                    }
                }

                // fallback: get all (non-paged) and do local filtering
                var all = await client.GetFromJsonAsync<List<BillVm>>("api/bills", jsonOptions) ?? new List<BillVm>();

                // If consumer -> call meters endpoint and filter by meter serial numbers (as original code)
                if (consumerId.HasValue)
                {
                    var meters = await client.GetFromJsonAsync<List<MeterViewModel>>($"api/meters?consumerId={consumerId.Value}", jsonOptions) ?? new List<MeterViewModel>();
                    var meterIds = new HashSet<string>(meters.Select(m => m.MeterSerialNo));
                    all = all.Where(b => meterIds.Contains(b.MeterID)).ToList();
                }

                // apply filters
                if (!string.IsNullOrWhiteSpace(status))
                    all = all.Where(b => string.Equals(b.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrWhiteSpace(meterId))
                    all = all.Where(b => string.Equals(b.MeterID, meterId, StringComparison.OrdinalIgnoreCase)).ToList();

                if (from.HasValue)
                    all = all.Where(b => b.MonthStartDate >= from.Value.Date).ToList();

                if (to.HasValue)
                    all = all.Where(b => b.MonthStartDate <= to.Value.Date).ToList();

                // ordering and paging
                all = all.OrderByDescending(b => b.MonthStartDate).ToList();
                var totalCount = all.Count;
                var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.Total = totalCount;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

                ViewBag.StatusFilter = status ?? "";
                ViewBag.MeterFilter = meterId ?? "";
                ViewBag.FromFilter = from?.ToString("yyyy-MM-dd") ?? "";
                ViewBag.ToFilter = to?.ToString("yyyy-MM-dd") ?? "";

                return View(paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load bills");
                TempData["err"] = "Unable to load bills: " + ex.Message;
                return View(new List<BillVm>());
            }
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

        private class BillsPagedResponse
        {
            public int Total { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public List<BillVm>? Items { get; set; }
        }

        // Minimal meter view model to support consumer-meter filtering
        private class MeterViewModel
        {
            public string MeterSerialNo { get; set; } = "";
            public int MeterId { get; set; }
        }
    }
}
