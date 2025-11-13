namespace AMIProjectView.Models
{
    public class UserVm
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string Status { get; set; } = "Active";

        public bool IsActive =>
            string.Equals(Status, "Active", StringComparison.OrdinalIgnoreCase);

        public DateTime? LastLogin { get; set; }
        public string? Password { get; set; }
    }
}
