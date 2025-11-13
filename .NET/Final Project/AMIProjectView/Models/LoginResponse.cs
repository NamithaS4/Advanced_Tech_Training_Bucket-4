namespace AMIProjectView.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string? Username { get; set; }
        public int? ConsumerId { get; set; }
    }
}
