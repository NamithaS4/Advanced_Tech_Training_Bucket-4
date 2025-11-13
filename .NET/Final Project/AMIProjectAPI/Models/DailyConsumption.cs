using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class DailyConsumption
{
    public string MeterId { get; set; } = null!;

    public DateOnly ConsumptionDate { get; set; }

    public decimal ConsumptionkWh { get; set; }

    public virtual Meter Meter { get; set; } = null!;
}
