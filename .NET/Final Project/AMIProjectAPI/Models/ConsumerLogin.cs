using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class ConsumerLogin
{
    public int ConsumerLoginId { get; set; }

    public int ConsumerId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? LastLogin { get; set; }

    public bool? IsVerified { get; set; }

    public string? Status { get; set; }

    public virtual Consumer Consumer { get; set; } = null!;
}
