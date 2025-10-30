using CollegeAppAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollegeAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CollegeDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(CollegeDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == login.Username && u.Password == login.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            // ✅ Generate JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
