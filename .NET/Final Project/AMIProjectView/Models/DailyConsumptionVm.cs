namespace AMIProjectView.Models
{
    public class DailyConsumptionVm
    {
        public string MeterId { get; set; } = "";
        public DateTime ConsumptionDate { get; set; }
        public decimal ConsumptionkWh { get; set; }
    }
}
