namespace AMIProjectView.Models
{
    public class ConsumerFormVm
    {
        public int? ConsumerId { get; set; }   // only for Update
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Status { get; set; } = "Active";  // Active/Inactive
        public int TariffId { get; set; } = 0;          // optional (keep if you use it)
    }

    public class ConsumerRowVm
    {
        public int ConsumerId { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Status { get; set; } = "Active";
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ConsumerPageVm
    {
        public ConsumerFormVm Form { get; set; } = new();
        public List<ConsumerRowVm> Items { get; set; } = new();
    }
}
