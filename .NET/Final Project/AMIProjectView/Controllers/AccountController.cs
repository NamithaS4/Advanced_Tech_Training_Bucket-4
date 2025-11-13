using Microsoft.AspNetCore.Mvc;
using AMIProjectView.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json.Linq;
using System.Net;

namespace AMIProjectView.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IHttpClientFactory httpClientFactory, ILogger<AccountController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginRequest());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("api");

            HttpResponseMessage resp = null;
            string finalLoginType = string.Empty;

            // --- 1) Attempt login-user ---
            try
            {
                resp = await client.PostAsJsonAsync("api/auth/login-user", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API login-user connectivity error.");
                ModelState.AddModelError("", "Unable to contact authentication server.");
                return View(model);
            }

            // If user login succeeded, mark and continue
            if (resp.IsSuccessStatusCode)
            {
                finalLoginType = "user";
            }
            else
            {
                // Read body of user response (if any)
                string userRespBody = string.Empty;
                try { userRespBody = await resp.Content.ReadAsStringAsync(); } catch { userRespBody = string.Empty; }

                // Extract the error (if present)
                string userError = ExtractApiError(userRespBody);

                // If API returned a clear inactive/verification/other informative message, stop and show it
                // We stop when the message is NOT the generic "Invalid username or password".
                if (!string.IsNullOrWhiteSpace(userError) &&
                    !userError.Equals("Invalid username or password", StringComparison.OrdinalIgnoreCase))
                {
                    // Preserve the specific API error (like "User is inactive")
                    ModelState.AddModelError("", userError);
                    _logger.LogWarning("Login-user returned specific error for {User}: {Error}", model.Username, userError);
                    return View(model);
                }

                // If the user error is generic invalid creds (or empty), only then attempt consumer login
                if (resp.StatusCode == HttpStatusCode.Unauthorized || resp.StatusCode == HttpStatusCode.BadRequest)
                {
                    try
                    {
                        resp = await client.PostAsJsonAsync("api/auth/login-consumer", model);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "API login-consumer connectivity error.");
                        ModelState.AddModelError("", "Unable to contact authentication server.");
                        return View(model);
                    }
                }
                else
                {
                    // For any other unexpected status code, show the API message (if any) or the reason phrase
                    var display = !string.IsNullOrWhiteSpace(userError) ? userError : $"{resp.ReasonPhrase}";
                    ModelState.AddModelError("", display);
                    _logger.LogWarning("Login-user failed with unexpected status {Status} and message {Msg}", resp.StatusCode, userRespBody);
                    return View(model);
                }
            }

            // --- After the above, resp now refers to either the successful user response OR the consumer response attempt ---
            if (finalLoginType == string.Empty && !resp.IsSuccessStatusCode)
            {
                // We tried consumer (because user returned generic invalid creds) but consumer also failed.
                // Read consumer body and surface specific errors if present.
                string consumerBody = string.Empty;
                try { consumerBody = await resp.Content.ReadAsStringAsync(); } catch { consumerBody = string.Empty; }

                string consumerError = ExtractApiError(consumerBody);

                // If consumer returned a specific message (like inactive / not verified), show that.
                if (!string.IsNullOrWhiteSpace(consumerError))
                {
                    ModelState.AddModelError("", consumerError);
                    _logger.LogWarning("login-consumer returned error for {User}: {Error}", model.Username, consumerError);
                    return View(model);
                }

                // Fallback: show reason phrase + raw body
                var fallback = !string.IsNullOrWhiteSpace(consumerBody) ? consumerBody : resp.ReasonPhrase;
                ModelState.AddModelError("", $"Login failed: {fallback}");
                _logger.LogWarning("Both login attempts failed for {User}. Status: {Status}. Body: {Body}", model.Username, resp.StatusCode, consumerBody);
                return View(model);
            }

            // If got here and finalLoginType is still empty but resp is success -> consumer succeeded
            if (finalLoginType == string.Empty && resp.IsSuccessStatusCode)
            {
                finalLoginType = "consumer";
            }

            // --- Success: read login response and sign in ---
            var loginResp = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResp == null || string.IsNullOrWhiteSpace(loginResp.Token))
            {
                ModelState.AddModelError("", "Login failed: no token returned.");
                return View(model);
            }

            HttpContext.Session.SetString("ApiToken", loginResp.Token);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim("UserType", finalLoginType == "consumer" ? "Consumer" : "User"),
                new Claim("access_token", loginResp.Token)
            };

            if (loginResp.ConsumerId.HasValue)
                claims.Add(new Claim("ConsumerId", loginResp.ConsumerId.Value.ToString()));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true, AllowRefresh = true });

            return finalLoginType == "consumer"
                ? RedirectToAction("Index", "Consumer")
                : RedirectToAction("Index", "Home");
        }

        // Helper: extract "error" string from API JSON. If not JSON or no error property, returns empty string.
        private string ExtractApiError(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return string.Empty;

            try
            {
                var obj = JObject.Parse(json);
                if (obj["error"] != null)
                    return obj["error"]!.ToString();
            }
            catch
            {
                // not JSON - ignore
            }

            return string.Empty;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
