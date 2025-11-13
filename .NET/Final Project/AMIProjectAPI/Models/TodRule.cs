using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class TodRule
{
    public int TodRuleId { get; set; }

    public string Name { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string PeakType { get; set; } = null!;

    public decimal Multiplier { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
