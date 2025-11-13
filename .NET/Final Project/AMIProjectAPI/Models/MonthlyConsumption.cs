using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class MonthlyConsumption
{
    public string MeterId { get; set; } = null!;

    public DateOnly MonthStartDate { get; set; }

    public decimal ConsumptionkWh { get; set; }

    public virtual Meter Meter { get; set; } = null!;
}
