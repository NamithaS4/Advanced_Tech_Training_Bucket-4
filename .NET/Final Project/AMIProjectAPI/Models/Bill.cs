using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class Bill
{
    public int BillId { get; set; }

    public string MeterId { get; set; } = null!;

    public DateOnly MonthStartDate { get; set; }

    public decimal MonthlyConsumptionkWh { get; set; }

    public string Category { get; set; } = null!;

    public decimal BaseRate { get; set; }

    public decimal SlabRate { get; set; }

    public decimal TaxRate { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime GeneratedAt { get; set; }

    public virtual Meter Meter { get; set; } = null!;
}
