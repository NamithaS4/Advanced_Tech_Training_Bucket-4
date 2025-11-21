using System.ComponentModel.DataAnnotations;

namespace AMIProjectView.Models
{
    public class AccountCreateVm
    {
        [Required]
        public string AccountType { get; set; } = "User"; // "User" or "Consumer"

        // Common fields
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Status { get; set; } = "Active";

        // User-specific fields
        public string? DisplayName { get; set; }

        // Consumer-specific fields
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}



