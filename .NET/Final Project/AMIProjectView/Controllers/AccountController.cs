using Microsoft.AspNetCore.Mvc;
using AMIProjectView.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;
using System.Net.Http.Headers;

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

            HttpResponseMessage respUser;
            HttpResponseMessage respConsumer = null;
            string finalLoginType = string.Empty;

            // --- 1) Attempt login-user ---
            try
            {
                respUser = await client.PostAsJsonAsync("api/auth/login-user", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API login-user connectivity error.");
                ModelState.AddModelError("", "Unable to contact authentication server.");
                return View(model);
            }

            // Read user response body safely
            string userRespBody = string.Empty;
            try { userRespBody = await respUser.Content.ReadAsStringAsync(); } catch { userRespBody = string.Empty; }

            // If user login succeeded, treat as user and continue
            if (respUser.IsSuccessStatusCode)
            {
                finalLoginType = "user";
                respConsumer = null;
            }
            else
            {
                // Try to extract a meaningful API error (supporting { error: "..."} or { message: "..." } or plain text)
                var userError = ExtractApiError(userRespBody);

                // If userError is specific (not the generic invalid credentials) show and stop
                if (!string.IsNullOrWhiteSpace(userError) &&
                    !userError.Equals("Invalid username or password", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", userError);
                    _logger.LogWarning("Login-user returned specific error for {User}: {Error}", model.Username, userError);
                    return View(model);
                }

                // Attempt consumer login
                try
                {
                    respConsumer = await client.PostAsJsonAsync("api/auth/login-consumer", model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "API login-consumer connectivity error.");
                    ModelState.AddModelError("", "Unable to contact authentication server.");
                    return View(model);
                }

                // Read consumer response body safely
                string consumerRespBody = string.Empty;
                try { consumerRespBody = await respConsumer.Content.ReadAsStringAsync(); } catch { consumerRespBody = string.Empty; }

                if (respConsumer.IsSuccessStatusCode)
                {
                    finalLoginType = "consumer";
                }
                else
                {
                    // Extract specific error and show it if present
                    var consumerError = ExtractApiError(consumerRespBody);
                    if (!string.IsNullOrWhiteSpace(consumerError))
                    {
                        ModelState.AddModelError("", consumerError);
                        _logger.LogWarning("login-consumer returned error for {User}: {Error}", model.Username, consumerError);
                        return View(model);
                    }

                    // Fallback: show consumer body or user body or generic message
                    var fallback = !string.IsNullOrWhiteSpace(consumerRespBody) ? consumerRespBody
                                  : (!string.IsNullOrWhiteSpace(userRespBody) ? userRespBody : "Invalid username or password");
                    ModelState.AddModelError("", $"Login failed: {fallback}");
                    _logger.LogWarning("Both login attempts failed for {User}. UserStatus: {UserStatus}, ConsumerStatus: {ConsStatus}. UserBody: {UserBody}, ConsBody: {ConsBody}",
                        model.Username, respUser.StatusCode, respConsumer.StatusCode, userRespBody, consumerRespBody);
                    return View(model);
                }
            }

            // Choose the successful response
            HttpResponseMessage successResp = finalLoginType == "user" ? respUser : respConsumer;

            if (successResp == null || !successResp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Login failed: unexpected error.");
                _logger.LogError("Login flow reached unexpected state for user {User}. finalLoginType={Type}", model.Username, finalLoginType);
                return View(model);
            }

            // Parse login response into LoginResponse
            LoginResponse? loginResp = null;
            try
            {
                var body = await successResp.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(body))
                {
                    loginResp = JsonSerializer.Deserialize<LoginResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse login response for {User}", model.Username);
            }

            if (loginResp == null || string.IsNullOrWhiteSpace(loginResp.Token))
            {
                ModelState.AddModelError("", "Login failed: no token returned.");
                _logger.LogWarning("Login succeeded but no token returned for {User}. Response: {Resp}", model.Username, successResp);
                return View(model);
            }

            // --- Defensive check for consumer: ensure the consumer is actually allowed to use consumer-scoped APIs ---
            if (finalLoginType == "consumer")
            {
                // create a temporary client and attach the token, then call a consumer-scoped endpoint.
                try
                {
                    var temp = _httpClientFactory.CreateClient("api");
                    temp.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResp.Token);

                    // call a consumer-scoped read endpoint that should return 200 for active consumers
                    // (we use /api/consumers/me/meters because it's safe and requires the token to be valid + consumer active)
                    var verifyResp = await temp.GetAsync("api/consumers/me/meters");

                    if (verifyResp.StatusCode == HttpStatusCode.Unauthorized || verifyResp.StatusCode == HttpStatusCode.Forbidden)
                    {
                        // Try to extract error body for a friendly message
                        string verifyBody = string.Empty;
                        try { verifyBody = await verifyResp.Content.ReadAsStringAsync(); } catch { verifyBody = string.Empty; }

                        var msg = ExtractApiError(verifyBody);
                        if (string.IsNullOrWhiteSpace(msg))
                        {
                            // fallback messages
                            msg = verifyResp.StatusCode == HttpStatusCode.Forbidden ? "Consumer is inactive or not authorized." : "Not authenticated (token rejected).";
                        }

                        ModelState.AddModelError("", msg);
                        _logger.LogWarning("Consumer login token rejected during verification for {User}: {Status} {Body}", model.Username, verifyResp.StatusCode, verifyBody);
                        return View(model);
                    }

                    // If verifyResp is not success but not 401/403, still treat as error
                    if (!verifyResp.IsSuccessStatusCode)
                    {
                        string text = string.Empty;
                        try { text = await verifyResp.Content.ReadAsStringAsync(); } catch { text = string.Empty; }
                        var msg = ExtractApiError(text);
                        if (string.IsNullOrWhiteSpace(msg)) msg = $"Unable to verify consumer account ({(int)verifyResp.StatusCode}).";
                        ModelState.AddModelError("", msg);
                        _logger.LogWarning("Consumer verification call failed for {User}: {Status} {Body}", model.Username, verifyResp.StatusCode, text);
                        return View(model);
                    }

                    // success -> continue
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error verifying consumer after token issuance for {User}", model.Username);
                    // conservative approach: don't allow login if verification fails due to connectivity
                    ModelState.AddModelError("", "Unable to verify consumer account. Please try again later.");
                    return View(model);
                }
            }

            // Save token in session and sign-in user
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

        // Helper: extract "error" or "message" string from API JSON or plain text.
        private string ExtractApiError(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return "";

            // Try JSON first
            try
            {
                var jobj = JObject.Parse(body);
                if (jobj["error"] != null)
                    return jobj["error"]!.ToString();
                if (jobj["message"] != null)
                    return jobj["message"]!.ToString();
            }
            catch
            {
                // Not JSON -> treat body itself as the error
            }

            // NEW: if API returned plain text like "Consumer is inactive"
            var trimmed = body.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
                return trimmed;

            return "";
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
