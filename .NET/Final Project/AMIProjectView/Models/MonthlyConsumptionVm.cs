namespace AMIProjectView.Models
{
    public class MonthlyConsumptionVm
    {
        public string MeterId { get; set; } = "";
        public DateTime MonthStartDate { get; set; }
        public decimal ConsumptionkWh { get; set; }
    }
}
