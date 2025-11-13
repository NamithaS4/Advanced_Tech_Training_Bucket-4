using System;

namespace AMIProjectView.Models
{
    public class BillVm
    {
        public int BillId { get; set; }
        public string MeterID { get; set; } = "";
        public DateTime MonthStartDate { get; set; }
        public decimal MonthlyConsumptionkWh { get; set; }
        public string Category { get; set; } = "";
        public decimal BaseRate { get; set; }
        public decimal SlabRate { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "";
        public DateTime GeneratedAt { get; set; }
    }
}
