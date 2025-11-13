using AMIProjectAPI.Models;
using AMIProjectAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BCrypt.Net;

namespace AMIProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AmiprojectContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AmiprojectContext context, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Username and password are required." });

            var usernameNormalized = req.Username.Trim();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == usernameNormalized.ToLower());

            // If user not found -> generic invalid credentials
            if (user == null)
                return Unauthorized(new { error = "Invalid username or password" });

            // --- Move password verification BEFORE status check ---
            bool verified = false;
            try
            {
                if (!string.IsNullOrEmpty(user.Password) && user.Password.StartsWith("$2"))
                {
                    // bcrypt-hashed stored password
                    verified = BCrypt.Net.BCrypt.Verify(req.Password, user.Password);
                }
                else
                {
                    // legacy/plain text
                    verified = req.Password == user.Password;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error verifying password for user {User}", req.Username);
                verified = false;
            }

            // If password invalid -> generic invalid credentials
            if (!verified)
                return Unauthorized(new { error = "Invalid username or password" });

            // Now check status after confirming password is correct
            if (!user.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { error = "User is inactive" });

            // update last login timestamp (we persist this)
            user.LastLogin = DateTime.UtcNow;
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update LastLogin for user {User}", user.Username);
                // don't block login on last-login persistence failure
            }

            var claims = new List<Claim>
            {
                new Claim("UserType", "User"),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.UserId.ToString())
            };

            var token = _tokenService.CreateToken(claims);
            return Ok(new { token });
        }

        [HttpPost("login-consumer")]
        public async Task<IActionResult> LoginConsumer([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "Username and password are required." });

            var usernameNormalized = req.Username.Trim();

            var cl = await _context.ConsumerLogins
                .FirstOrDefaultAsync(x => x.Username.ToLower() == usernameNormalized.ToLower());

            if (cl == null)
                return Unauthorized(new { error = "Invalid username or password" });

            // --- Password verification first ---
            bool verified = false;
            try
            {
                if (!string.IsNullOrEmpty(cl.Password) && cl.Password.StartsWith("$2"))
                {
                    verified = BCrypt.Net.BCrypt.Verify(req.Password, cl.Password);
                }
                else
                {
                    verified = req.Password == cl.Password;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error verifying consumer password for {User}", req.Username);
                verified = false;
            }

            if (!verified)
                return Unauthorized(new { error = "Invalid username or password" });

            // Now check status and verification only after password is correct
            if (!cl.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { error = "Consumer is inactive" });

            if (cl.IsVerified == false)
                return Unauthorized(new { error = "Consumer not verified" });

            cl.LastLogin = DateTime.UtcNow;
            try
            {
                _context.ConsumerLogins.Update(cl);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update LastLogin for consumer {User}", cl.Username);
            }

            var claims = new List<Claim>
            {
                new Claim("UserType", "Consumer"),
                new Claim(ClaimTypes.Name, cl.Username),
                new Claim("ConsumerId", cl.ConsumerId.ToString())
            };

            var token = _tokenService.CreateToken(claims);
            return Ok(new { token });
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
