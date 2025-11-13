using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace AMIProjectView.Controllers
{
    // allow any authenticated principal to access read pages; writes are restricted per-action
    [Authorize]
    public class MeterController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MeterController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MeterController(IHttpClientFactory httpClientFactory, ILogger<MeterController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        // ------------------------------------------------------------
        // helper to attach bearer token (kept from your original file)
        // ------------------------------------------------------------
        private void AddBearer(HttpClient client)
        {
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // ------------------------------------------------------------
        // LIST (available to User and Consumer)
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                // Try to detect consumer id if the principal is a consumer
                var isConsumer = User.HasClaim(c => c.Type == "UserType" && c.Value == "Consumer");
                int? consumerId = null;
                if (isConsumer)
                {
                    consumerId = TryGetConsumerIdFromClaims();
                }

                // Prefer server-side filtering if API supports it (API will honor consumer claim)
                if (consumerId.HasValue)
                {
                    try
                    {
                        // Request the API; API returns only consumer meters if token belongs to consumer.
                        var resp = await client.GetAsync($"api/meters?page={page}&pageSize={pageSize}");
                        if (resp.IsSuccessStatusCode)
                        {
                            // try to parse paged wrapper { total, page, pageSize, items }
                            try
                            {
                                using var stream = await resp.Content.ReadAsStreamAsync();
                                var wrapper = await JsonSerializer.DeserializeAsync<MetersPagedResponse?>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                if (wrapper != null && wrapper.Items != null)
                                {
                                    ViewBag.Total = wrapper.Total;
                                    ViewBag.Page = wrapper.Page;
                                    ViewBag.PageSize = wrapper.PageSize;
                                    ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(wrapper.Total / (double)wrapper.PageSize));
                                    return View(wrapper.Items);
                                }
                            }
                            catch { /* ignore and try array below */ }

                            // maybe API returned array
                            try
                            {
                                var arr = await resp.Content.ReadFromJsonAsync<List<MeterViewModel>>();
                                if (arr != null)
                                {
                                    ViewBag.Total = arr.Count;
                                    ViewBag.Page = page;
                                    ViewBag.PageSize = pageSize;
                                    ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling((double)arr.Count / pageSize));
                                    var items = arr.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                                    return View(items);
                                }
                            }
                            catch { /* fallback below */ }
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogWarning(ex, "API request for consumer meters failed, will fallback to full fetch.");
                    }
                }

                // fallback: fetch all meters then filter client-side if consumer
                var all = await client.GetFromJsonAsync<List<MeterViewModel>>("api/meters") ?? new List<MeterViewModel>();

                if (consumerId.HasValue)
                    all = all.Where(m => m.ConsumerId == consumerId.Value).ToList();

                // stable order by MeterSerialNo
                all = all.OrderBy(m => m.MeterSerialNo).ToList();

                var total = all.Count;
                var itemsPaged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.Total = total;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

                return View(itemsPaged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading meters");
                ViewBag.Error = "Unable to load meters from API.";
                return View(new List<MeterViewModel>());
            }
        }

        // ------------------------------------------------------------
        // CREATE (admin only)
        // ------------------------------------------------------------
        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult Create()
        {
            return View(new MeterEditViewModel
            {
                Status = "Active",
                Category = "Residential Tariff",
                InstallDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Create(MeterEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            var payload = new
            {
                MeterSerialNo = vm.MeterSerialNo,
                ConsumerId = vm.ConsumerId,
                OrgUnitId = vm.OrgUnitId,
                Ipaddress = vm.IpAddress,
                Iccid = vm.ICCID,
                Imsi = vm.IMSI,
                Manufacturer = vm.Manufacturer,
                Firmware = vm.Firmware,
                Category = vm.Category,
                InstallDate = vm.InstallDate.ToString("yyyy-MM-dd"),
                Status = vm.Status
            };

            try
            {
                var resp = await client.PostAsJsonAsync("api/meters", payload);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["msg"] = "Meter added successfully.";
                    return RedirectToAction(nameof(Index));
                }

                var parsed = await ParseApiErrorAsync(resp);
                if (parsed != null)
                {
                    if (!string.IsNullOrEmpty(parsed.Field))
                    {
                        var vmField = MapApiFieldToVm(parsed.Field);
                        if (!string.IsNullOrEmpty(vmField))
                            ModelState.AddModelError(vmField, parsed.Message);
                        else
                            ModelState.AddModelError(string.Empty, parsed.Message);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, parsed.Message);
                    }
                }
                else
                {
                    var raw = await resp.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Create failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. {raw}");
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create meter failed");
                ModelState.AddModelError(string.Empty, "Create failed: " + ex.Message);
                return View(vm);
            }
        }

        // ------------------------------------------------------------
        // EDIT (admin only)
        // ------------------------------------------------------------
        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Edit(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
                return RedirectToAction(nameof(Index));

            try
            {
                var client = _httpClientFactory.CreateClient("api");
                AddBearer(client);
                var m = await client.GetFromJsonAsync<MeterEditViewModel>($"api/meters/{serial}");
                if (m == null)
                {
                    TempData["err"] = "Meter not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(m);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load meter failed");
                TempData["err"] = "Unable to load meter.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Edit(MeterEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var client = _httpClientFactory.CreateClient("api");
                AddBearer(client);

                var payload = new
                {
                    MeterSerialNo = vm.MeterSerialNo,
                    ConsumerId = vm.ConsumerId,
                    OrgUnitId = vm.OrgUnitId,
                    Ipaddress = vm.IpAddress,
                    Iccid = vm.ICCID,
                    Imsi = vm.IMSI,
                    Manufacturer = vm.Manufacturer,
                    Firmware = vm.Firmware,
                    Category = vm.Category,
                    InstallDate = vm.InstallDate.ToString("yyyy-MM-dd"),
                    Status = vm.Status
                };

                var resp = await client.PutAsJsonAsync($"api/meters/{vm.MeterSerialNo}", payload);
                if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    TempData["msg"] = "Meter updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                var parsed = await ParseApiErrorAsync(resp);
                if (parsed != null)
                {
                    var vmField = !string.IsNullOrEmpty(parsed.Field) ? MapApiFieldToVm(parsed.Field) : null;
                    if (!string.IsNullOrEmpty(vmField))
                        ModelState.AddModelError(vmField, parsed.Message);
                    else
                        ModelState.AddModelError(string.Empty, parsed.Message);
                }
                else
                {
                    var raw = await resp.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Update failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. {raw}");
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update meter failed");
                ModelState.AddModelError(string.Empty, "Update failed: " + ex.Message);
                return View(vm);
            }
        }

        // ------------------------------------------------------------
        // DELETE (soft decommission) - admin only
        // ------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Delete(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
                return RedirectToAction(nameof(Index));

            try
            {
                var client = _httpClientFactory.CreateClient("api");
                AddBearer(client);
                var resp = await client.DeleteAsync($"api/meters/{serial}");
                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    TempData["err"] = $"Delete failed. {resp.StatusCode}: {body}";
                }
                else
                {
                    TempData["msg"] = "Meter decommissioned.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete meter failed");
                TempData["err"] = "Delete failed: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ------------------------------------------------------------
        // UPLOAD CSV (admin only)
        // ------------------------------------------------------------
        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult Upload() => View();

        [HttpGet]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult TemplateCsv()
        {
            var csv =
                "MeterSerialNo,ConsumerId,OrgUnitId,IPAddress,ICCID,IMSI,Manufacturer,Firmware,Category,InstallDate,Status\r\n" +
                "MTR5001,1,1,192.168.1.50,ICCID5001,IMSI5001,Siemens,1.0.0,Residential Tariff,2025-09-22,Active\r\n";
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "meter_template.csv");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserPolicy")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["err"] = "Please choose a CSV file.";
                return View();
            }

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                TempData["err"] = "Only .csv files are accepted.";
                return View();
            }

            var ok = 0;
            var fail = 0;
            var errors = new List<string>();
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                var header = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(header))
                {
                    TempData["err"] = "The file is empty.";
                    return View();
                }

                int lineNo = 1;
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNo++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = line.Split(',');
                    if (cols.Length < 11)
                    {
                        fail++;
                        errors.Add($"Line {lineNo}: not enough columns.");
                        continue;
                    }

                    var dto = new
                    {
                        MeterSerialNo = cols[0].Trim(),
                        ConsumerId = TryInt(cols[1], out var cId) ? cId : (int?)null,
                        OrgUnitId = TryInt(cols[2], out var ouId) ? ouId : (int?)null,
                        Ipaddress = cols[3].Trim(),
                        Iccid = cols[4].Trim(),
                        Imsi = cols[5].Trim(),
                        Manufacturer = cols[6].Trim(),
                        Firmware = cols[7].Trim(),
                        Category = NormalizeCategory(cols[8]),
                        InstallDate = TryDate(cols[9], out var dt) ? dt.ToString("yyyy-MM-dd") : null,
                        Status = NormalizeStatus(cols[10])
                    };

                    var missing = new List<string>();
                    if (string.IsNullOrWhiteSpace(dto.MeterSerialNo)) missing.Add("MeterSerialNo");
                    if (dto.ConsumerId == null) missing.Add("ConsumerId");
                    if (dto.OrgUnitId == null) missing.Add("OrgUnitId");
                    if (string.IsNullOrWhiteSpace(dto.Ipaddress)) missing.Add("IPAddress");
                    if (string.IsNullOrWhiteSpace(dto.Iccid)) missing.Add("ICCID");
                    if (string.IsNullOrWhiteSpace(dto.Imsi)) missing.Add("IMSI");
                    if (string.IsNullOrWhiteSpace(dto.Manufacturer)) missing.Add("Manufacturer");
                    if (string.IsNullOrWhiteSpace(dto.Category)) missing.Add("Category");
                    if (string.IsNullOrWhiteSpace(dto.InstallDate)) missing.Add("InstallDate");
                    if (string.IsNullOrWhiteSpace(dto.Status)) missing.Add("Status");

                    if (missing.Count > 0)
                    {
                        fail++;
                        errors.Add($"Line {lineNo}: missing {string.Join(", ", missing)}");
                        continue;
                    }

                    var resp = await client.PostAsJsonAsync("api/meters", dto);
                    if (resp.IsSuccessStatusCode)
                    {
                        ok++;
                    }
                    else
                    {
                        fail++;
                        var parsed = await ParseApiErrorAsync(resp);
                        if (parsed != null)
                            errors.Add($"Line {lineNo}: API error {resp.StatusCode}. {parsed.Message}");
                        else
                        {
                            var body = await resp.Content.ReadAsStringAsync();
                            errors.Add($"Line {lineNo}: API error {resp.StatusCode}. {body}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload failed");
                TempData["err"] = "Upload failed: " + ex.Message;
                return View();
            }

            TempData["msg"] = $"Upload complete. Imported: {ok}, Failed: {fail}.";
            if (errors.Any())
            {
                TempData["err"] = string.Join("<br/>", errors.Take(8));
            }

            return RedirectToAction(nameof(Index));
        }

        // ------------------------------------------------------------
        // helpers (unchanged)
        // ------------------------------------------------------------
        private static bool TryInt(string s, out int value)
            => int.TryParse(s?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

        private static bool TryDate(string s, out DateTime dt)
        {
            s = s?.Trim() ?? "";
            return DateTime.TryParseExact(
                s,
                new[] { "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "MM/dd/yyyy", "dd/MM/yyyy" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dt);
        }

        private static string NormalizeStatus(string s)
        {
            s = (s ?? "").Trim();
            return s.Equals("Active", StringComparison.OrdinalIgnoreCase) ? "Active"
                 : s.Equals("Inactive", StringComparison.OrdinalIgnoreCase) ? "Inactive"
                 : s.Equals("Decommissioned", StringComparison.OrdinalIgnoreCase) ? "Decommissioned"
                 : "";
        }

        private static string NormalizeCategory(string s)
        {
            s = (s ?? "").Trim();
            return s.Equals("Residential Tariff", StringComparison.OrdinalIgnoreCase) ? "Residential Tariff"
                 : s.Equals("Commercial Tariff", StringComparison.OrdinalIgnoreCase) ? "Commercial Tariff"
                 : s.Equals("Factory Tariff", StringComparison.OrdinalIgnoreCase) ? "Factory Tariff"
                 : "";
        }

        private async Task<ApiError?> ParseApiErrorAsync(HttpResponseMessage resp)
        {
            try
            {
                var str = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(str)) return null;

                using var doc = JsonDocument.Parse(str);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    // look for { field, message }
                    if (root.TryGetProperty("field", out var pField) && root.TryGetProperty("message", out var pMsg))
                    {
                        return new ApiError { Field = pField.GetString(), Message = pMsg.GetString() ?? "" };
                    }

                    // look for { error: "..." } or { Message: "..." }
                    if (root.TryGetProperty("error", out var perr) && perr.ValueKind == JsonValueKind.String)
                    {
                        return new ApiError { Message = perr.GetString() ?? "" };
                    }
                    if (root.TryGetProperty("Message", out var pMessage) && pMessage.ValueKind == JsonValueKind.String)
                    {
                        return new ApiError { Message = pMessage.GetString() ?? "" };
                    }

                    // sometimes API returns validation dictionary
                    foreach (var prop in root.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array && prop.Value.GetArrayLength() > 0)
                        {
                            var first = prop.Value[0];
                            if (first.ValueKind == JsonValueKind.String)
                                return new ApiError { Field = prop.Name, Message = first.GetString() ?? "" };
                        }
                        else if (prop.Value.ValueKind == JsonValueKind.String)
                        {
                            return new ApiError { Field = prop.Name, Message = prop.Value.GetString() ?? "" };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "ParseApiErrorAsync failed to parse response.");
            }

            return null;
        }

        private static string? MapApiFieldToVm(string apiField)
        {
            if (string.IsNullOrWhiteSpace(apiField)) return null;
            apiField = apiField.Trim();

            return apiField switch
            {
                "ConsumerId" => nameof(MeterEditViewModel.ConsumerId),
                "consumerId" => nameof(MeterEditViewModel.ConsumerId),
                "OrgUnitId" => nameof(MeterEditViewModel.OrgUnitId),
                "orgUnitId" => nameof(MeterEditViewModel.OrgUnitId),
                "MeterSerialNo" => nameof(MeterEditViewModel.MeterSerialNo),
                "meterSerialNo" => nameof(MeterEditViewModel.MeterSerialNo),
                "Ipaddress" => nameof(MeterEditViewModel.IpAddress),
                "IpAddress" => nameof(MeterEditViewModel.IpAddress),
                "IPAddress" => nameof(MeterEditViewModel.IpAddress),
                "ICCID" => nameof(MeterEditViewModel.ICCID),
                "ICcid" => nameof(MeterEditViewModel.ICCID),
                "IMSI" => nameof(MeterEditViewModel.IMSI),
                _ => null
            };
        }

        private int? TryGetConsumerIdFromClaims()
        {
            // 1) explicit claim "ConsumerId"
            var c = User.Claims.FirstOrDefault(x => string.Equals(x.Type, "ConsumerId", StringComparison.OrdinalIgnoreCase));
            if (c != null && int.TryParse(c.Value, out var cid)) return cid;

            // 2) nameidentifier (ClaimTypes.NameIdentifier)
            var nid = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(nid) && int.TryParse(nid, out var nidInt)) return nidInt;

            // 3) session fallback
            var s = _httpContextAccessor.HttpContext?.Session?.GetString("ConsumerId");
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var sid)) return sid;

            return null;
        }

        private class MetersPagedResponse
        {
            public int Total { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public List<MeterViewModel>? Items { get; set; }
        }

        private class ApiError
        {
            public string? Field { get; set; }
            public string Message { get; set; } = "";
        }
    }
}
