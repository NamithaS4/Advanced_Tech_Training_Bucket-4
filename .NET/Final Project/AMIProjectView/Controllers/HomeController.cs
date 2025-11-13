using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AMIProjectView.Controllers
{
    [Authorize(Policy = "UserPolicy")]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IHttpClientFactory httpClientFactory,
                              ILogger<HomeController> logger,
                              IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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
            var vm = new DashboardViewModel();

            try
            {
                var client = _httpClientFactory.CreateClient("api");
                AddBearer(client);

                // call the aggregated dashboard API
                _logger.LogInformation("Calling API dashboard at {Url}", new Uri(client.BaseAddress, "api/dashboard"));
                var apiResp = await client.GetFromJsonAsync<DashboardViewModel>("api/dashboard");

                if (apiResp == null)
                {
                    TempData["err"] = "Unable to load dashboard data from API: empty response.";
                    return View(vm);
                }

                // map simple counts
                vm.TotalMeters = apiResp.TotalMeters;
                vm.ActiveMeters = apiResp.ActiveMeters;
                vm.TotalConsumers = apiResp.TotalConsumers;
                vm.ActiveConsumers = apiResp.ActiveConsumers;
                vm.TotalUsers = apiResp.TotalUsers;
                vm.ActiveUsers = apiResp.ActiveUsers;
                vm.TariffPlans = apiResp.TariffPlans;
                vm.Manufacturers = apiResp.Manufacturers;
                vm.Dtrs = apiResp.Dtrs;

                // map recent consumers (the API should return { Name, Email, CreatedAt })
                vm.RecentConsumers = apiResp.RecentConsumers?
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .Select(c => new RecentConsumerVm
                    {
                        Name = c.Name ?? "",
                        Email = c.Email ?? "",
                        CreatedAt = c.CreatedAt
                    }).ToList() ?? new List<RecentConsumerVm>();

                // map recent users (the API should return { Username, Role, CreatedAt })
                List<RecentUserVm>? recentUserVms = apiResp.RecentUsers?
            .OrderByDescending(u => u.LastLogin ?? DateTime.MinValue)
            .Take(5)
            .Select(u => new RecentUserVm
            {
                Username = u.Username ?? "",
                DisplayName = u.DisplayName ?? "",
                LastLogin = u.LastLogin
            }).ToList();
                vm.RecentUsers = recentUserVms;
            }
            catch (HttpRequestException hx)
            {
                _logger.LogError(hx, "API request failed for dashboard: {Message}", hx.Message);
                TempData["err"] = $"Unable to load dashboard data from API: {hx.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in dashboard");
                TempData["err"] = "Unexpected error loading dashboard.";
            }

            return View(vm);
        }

    }
}
