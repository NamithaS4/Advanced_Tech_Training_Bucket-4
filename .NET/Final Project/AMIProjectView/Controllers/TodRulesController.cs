using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AMIProjectView.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    public class TodRulesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TodRulesController(IHttpClientFactory httpClientFactory,
                                  IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddBearer(HttpClient client)
        {
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);
            var data = await client.GetFromJsonAsync<List<TodRuleVm>>("api/todrules");
            return View(data ?? new List<TodRuleVm>());
        }

        [HttpGet]
        public IActionResult Create() => View(new TodRuleVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TodRuleVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);
            var resp = await client.PostAsJsonAsync("api/todrules", vm);
            if (!resp.IsSuccessStatusCode)
            {
                TempData["err"] = $"Create failed: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                return View(vm);
            }
            TempData["msg"] = "TOD Rule added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);
            var vm = await client.GetFromJsonAsync<TodRuleVm>($"api/todrules/{id}");
            if (vm == null) { TempData["err"] = "Rule not found."; return RedirectToAction(nameof(Index)); }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TodRuleVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);
            var resp = await client.PutAsJsonAsync($"api/todrules/{vm.TodRuleId}", vm);
            if (!resp.IsSuccessStatusCode)
            {
                TempData["err"] = $"Update failed: {(int)resp.StatusCode} {resp.ReasonPhrase}";
                return View(vm);
            }
            TempData["msg"] = "TOD Rule updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);
            var resp = await client.DeleteAsync($"api/todrules/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                TempData["err"] = $"Delete failed: {(int)resp.StatusCode} {resp.ReasonPhrase}";
            }
            else TempData["msg"] = "TOD Rule deactivated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
