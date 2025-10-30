using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollegeAppAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        // static login users since we don't want extra table
        private readonly Dictionary<string, string> _users = new()
        {
            { "admin", "admin123" },
            { "faculty", "faculty123" }
        };

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string? Authenticate(string username, string password)
        {
            if (!_users.Any(u => u.Key == username && u.Value == password))
                return null;

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
