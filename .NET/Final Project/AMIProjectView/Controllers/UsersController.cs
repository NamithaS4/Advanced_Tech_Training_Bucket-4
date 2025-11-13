using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace AMIProjectView.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IHttpClientFactory http, ILogger<UsersController> logger)
        {
            _http = http;
            _logger = logger;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var client = _http.CreateClient("api");
            var users = await client.GetFromJsonAsync<List<UserVm>>("api/users");
            return View(users ?? new List<UserVm>());
        }

        // GET: /Users/Create
        [HttpGet]
        public IActionResult Create()
        {
            ModelState.Clear();
            return View(new UserVm());
        }

        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _http.CreateClient("api");
            var payload = new
            {
                vm.Username,
                vm.DisplayName,
                vm.Email,
                vm.Phone,
                vm.Status,
                vm.Password
            };

            var resp = await client.PostAsJsonAsync("api/users", payload);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["err"] = $"Create failed: {body}";
                return View(vm);
            }

            TempData["msg"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _http.CreateClient("api");
            var vm = await client.GetFromJsonAsync<UserVm>($"api/users/{id}");
            if (vm == null) { TempData["err"] = "User not found."; return RedirectToAction(nameof(Index)); }
            return View(vm);
        }

        // POST: /Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _http.CreateClient("api");
            var payload = new { vm.DisplayName, vm.Email, vm.Phone, vm.Status };
            var resp = await client.PutAsJsonAsync($"api/users/{vm.UserId}", payload);

            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                TempData["err"] = $"Update failed: {body}";
                return View(vm);
            }

            TempData["msg"] = "User updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Users/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _http.CreateClient("api");
            await client.DeleteAsync($"api/users/{id}");
            TempData["msg"] = "User deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
