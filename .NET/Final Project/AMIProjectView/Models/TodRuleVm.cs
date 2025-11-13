namespace AMIProjectView.Models
{
    public class TodRuleVm
    {
        public int TodRuleId { get; set; }
        public string Name { get; set; } = "";
        public TimeSpan StartTime { get; set; }        // e.g. 06:00
        public TimeSpan EndTime { get; set; }          // e.g. 10:00
        public string PeakType { get; set; } = "";     // e.g. "Peak", "Offpeak"
        public decimal Multiplier { get; set; } = 1m;  // rate multiplier
        public string Status { get; set; } = "Active";
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
