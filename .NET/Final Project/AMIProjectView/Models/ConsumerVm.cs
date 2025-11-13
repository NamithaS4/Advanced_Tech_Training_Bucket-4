namespace AMIProjectView.Models
{
    public class ConsumerVm
    {
        public int ConsumerId { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        // numeric id used by create/edit API requests
        public int TariffId { get; set; } = 0;

        // NEW: credentials for creating consumer login (only used on Create)
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public string Status { get; set; } = "Active";
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
