using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMIProjectView.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    public class ConsumersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ConsumersController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsumersController(IHttpClientFactory httpClientFactory,
                                   ILogger<ConsumersController> logger,
                                   IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        // Attach Bearer token from session to outgoing API client
        private void AddBearer(HttpClient client)
        {
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // ---------------------------
        // INDEX (with pagination)
        // ---------------------------
        // GET: /Consumers?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                // fetch all and do server-side paging (API may support paging later)
                var all = await client.GetFromJsonAsync<List<ConsumerVm>>("api/consumers") ?? new List<ConsumerVm>();

                // ensure stable ordering by ConsumerId ascending so new records (higher ids) appear last
                all = all.OrderBy(c => c.ConsumerId).ToList();

                var total = all.Count;
                var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.Total = total;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load consumers");
                TempData["err"] = "Unable to load consumers: " + ex.Message;
                return View(new List<ConsumerVm>());
            }
        }

        // ---------------------------
        // CREATE
        // ---------------------------
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ConsumerVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsumerVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            var payload = new
            {
                Name = vm.Name,
                Address = vm.Address,
                Phone = vm.Phone,
                Email = vm.Email,
                Status = vm.Status,
                Username = string.IsNullOrWhiteSpace(vm.Username) ? null : vm.Username,
                Password = string.IsNullOrWhiteSpace(vm.Password) ? null : vm.Password
            };


            try
            {
                var resp = await client.PostAsJsonAsync("api/consumers", payload);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["msg"] = "Consumer added.";
                    // redirect to first page where the new consumer will appear (sorted by ConsumerId ascending it will be last)
                    return RedirectToAction(nameof(Index));
                }

                // not successful: try to parse structured API error
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
                _logger.LogError(ex, "Create failed");
                ModelState.AddModelError(string.Empty, "Create failed: " + ex.Message);
                return View(vm);
            }
        }

        // ---------------------------
        // EDIT
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                var vm = await client.GetFromJsonAsync<ConsumerVm>($"api/consumers/{id}");
                if (vm == null)
                {
                    TempData["err"] = "Consumer not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load consumer failed");
                TempData["err"] = "Unable to load consumer: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConsumerVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            var payload = new
            {
                Name = vm.Name,
                Address = vm.Address,
                Phone = vm.Phone,
                Email = vm.Email,
                Status = vm.Status
            };

            try
            {
                var resp = await client.PutAsJsonAsync($"api/consumers/{vm.ConsumerId}", payload);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["msg"] = "Consumer updated.";
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
                _logger.LogError(ex, "Update failed");
                ModelState.AddModelError(string.Empty, "Update failed: " + ex.Message);
                return View(vm);
            }
        }

        // ---------------------------
        // DELETE
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("api");
            await client.DeleteAsync($"api/consumers/{id}");
            return RedirectToAction(nameof(Index));
        }
        

        // ---------------------------
        // Helpers
        // ---------------------------
        private async Task<ApiError?> ParseApiErrorAsync(HttpResponseMessage resp)
        {
            try
            {
                var raw = await resp.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(raw)) return null;

                // Try parse JSON; if it fails we'll fall back to raw text below
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    var root = doc.RootElement;

                    // If top-level object { ... }
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        string? field = null;
                        var messages = new List<string>();

                        // common single-field shapes
                        if (root.TryGetProperty("field", out var pField) && pField.ValueKind == JsonValueKind.String)
                            field = pField.GetString();

                        if (root.TryGetProperty("message", out var pMsg))
                        {
                            if (pMsg.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(pMsg.GetString()))
                                messages.Add(pMsg.GetString()!);
                            else if (pMsg.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var it in pMsg.EnumerateArray())
                                    if (it.ValueKind == JsonValueKind.String) messages.Add(it.GetString()!);
                            }
                        }

                        // other common names
                        if (messages.Count == 0 && root.TryGetProperty("error", out var pErr))
                        {
                            if (pErr.ValueKind == JsonValueKind.String)
                                messages.Add(pErr.GetString()!);
                        }
                        if (messages.Count == 0 && root.TryGetProperty("Message", out var pMessage))
                        {
                            if (pMessage.ValueKind == JsonValueKind.String)
                                messages.Add(pMessage.GetString()!);
                        }
                        if (messages.Count == 0)
                        {
                            foreach (var prop in root.EnumerateObject())
                            {
                                // If property is an array of strings, add first string (and keep field name if obvious)
                                if (prop.Value.ValueKind == JsonValueKind.Array && prop.Value.GetArrayLength() > 0)
                                {
                                    var first = prop.Value[0];
                                    if (first.ValueKind == JsonValueKind.String)
                                        messages.Add(first.GetString()!);
                                    // record field if not yet set
                                    if (string.IsNullOrEmpty(field))
                                        field = prop.Name;
                                }
                                else if (prop.Value.ValueKind == JsonValueKind.String)
                                {
                                    // single string property error
                                    messages.Add(prop.Value.GetString()!);
                                    if (string.IsNullOrEmpty(field))
                                        field = prop.Name;
                                }
                            }
                        }

                        if (messages.Count > 0)
                        {
                            // join multiple messages into one readable string
                            var msg = string.Join("; ", messages.Select(m => m.Trim()).Where(m => !string.IsNullOrEmpty(m)));
                            return new ApiError { Field = field, Message = msg };
                        }

                        // If object but we couldn't extract, fall back to raw text
                        return new ApiError { Message = raw.Trim('"') };
                    }

                    // If top-level array: try to gather string entries
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        var list = new List<string>();
                        foreach (var el in root.EnumerateArray())
                        {
                            if (el.ValueKind == JsonValueKind.String)
                                list.Add(el.GetString()!);
                            else if (el.ValueKind == JsonValueKind.Object)
                            {
                                // prefer message property inside objects
                                if (el.TryGetProperty("message", out var p) && p.ValueKind == JsonValueKind.String)
                                    list.Add(p.GetString()!);
                            }
                        }

                        if (list.Count > 0)
                        {
                            var msg = string.Join("; ", list.Select(m => m.Trim()).Where(m => !string.IsNullOrEmpty(m)));
                            return new ApiError { Message = msg };
                        }
                    }

                    // If it's a JSON string literal: "Some text"
                    if (root.ValueKind == JsonValueKind.String)
                    {
                        var s = root.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                            return new ApiError { Message = s };
                    }
                }
                catch (JsonException je)
                {
                    // not JSON - continue to fallback
                    _logger.LogDebug(je, "ParseApiErrorAsync: body not JSON, falling back to raw text");
                }

                // fallback: return raw body text
                return new ApiError { Message = raw.Trim('"') };
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "ParseApiErrorAsync failed");
                return null;
            }
        }


        private static string? MapApiFieldToVm(string apiField)
        {
            if (string.IsNullOrWhiteSpace(apiField)) return null;
            apiField = apiField.Trim();

            return apiField switch
            {
                "Name" => nameof(ConsumerVm.Name),
                "Email" => nameof(ConsumerVm.Email),
                "Phone" => nameof(ConsumerVm.Phone),
                "Username" => nameof(ConsumerVm.Username),
                "username" => nameof(ConsumerVm.Username),
                _ => null
            };
        }


        private class ApiError
        {
            public string? Field { get; set; }
            public string Message { get; set; } = "";
        }
    }
}
