using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class Tariff
{
    public int TariffId { get; set; }

    public string TariffName { get; set; } = null!;

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public decimal BaseRate { get; set; }

    public decimal TaxRate { get; set; }

    public virtual ICollection<TariffSlab> TariffSlabs { get; set; } = new List<TariffSlab>();
}
