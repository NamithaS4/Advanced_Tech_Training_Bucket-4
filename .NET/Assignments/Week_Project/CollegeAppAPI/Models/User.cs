namespace CollegeAppAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // store plain for now
        public string? Role { get; set; }  // optional: "Admin", "Faculty", etc.
    }
}
