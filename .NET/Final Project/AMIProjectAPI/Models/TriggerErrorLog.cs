using System;
using System.Collections.Generic;

namespace AMIProjectAPI.Models;

public partial class TriggerErrorLog
{
    public int LogId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? TriggerName { get; set; }

    public string? ErrorMessage { get; set; }

    public int? ErrorNumber { get; set; }

    public string? ErrorProcedure { get; set; }

    public int? ErrorLine { get; set; }
}
