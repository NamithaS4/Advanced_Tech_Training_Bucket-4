namespace AMIProjectView.Models
{
    public class ConsumerAdminViewModel
    {
        public int ConsumerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int TariffId { get; set; }
        public string TariffName { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "admin";
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
