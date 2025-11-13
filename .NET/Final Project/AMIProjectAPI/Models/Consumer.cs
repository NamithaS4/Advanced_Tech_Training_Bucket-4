using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class Consumer
{
    public int ConsumerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ConsumerLogin? ConsumerLogin { get; set; }

    public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();
}
