using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class Meter
{
    public string MeterSerialNo { get; set; } = null!;

    public int ConsumerId { get; set; }

    public string Ipaddress { get; set; } = null!;

    public string Iccid { get; set; } = null!;

    public string Imsi { get; set; } = null!;

    public string Manufacturer { get; set; } = null!;

    public string? Firmware { get; set; }

    public string Category { get; set; } = null!;

    public int OrgUnitId { get; set; }

    public DateTime InstallDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual Consumer Consumer { get; set; } = null!;

    public virtual ICollection<MonthlyConsumption> MonthlyConsumptions { get; set; } = new List<MonthlyConsumption>();

    public virtual OrgUnit OrgUnit { get; set; } = null!;
}
