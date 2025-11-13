using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class OrgUnit
{
    public int OrgUnitId { get; set; }

    public string? Zone { get; set; }

    public string? Substation { get; set; }

    public string? Feeder { get; set; }

    public string? Dtr { get; set; }

    public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();
}
