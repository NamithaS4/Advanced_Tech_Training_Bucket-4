namespace AMIProjectAPI.DTOs
{
    public class UserCreateDto
    {
        public string Username { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; } = "Active";
        public string Password { get; set; } = "";
    }
}
