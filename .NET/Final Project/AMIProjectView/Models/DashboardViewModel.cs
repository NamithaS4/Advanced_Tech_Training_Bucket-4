// AMIProjectView/Models/DashboardViewModel.cs
using System;
using System.Collections.Generic;

namespace AMIProjectView.Models
{
    public class DashboardViewModel
    {
        public int TotalMeters { get; set; }
        public int ActiveMeters { get; set; }

        public int TotalConsumers { get; set; }
        public int ActiveConsumers { get; set; }

        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }

        public int TariffPlans { get; set; }
        public int Manufacturers { get; set; }
        public int Dtrs { get; set; }

        public List<RecentConsumerVm> RecentConsumers { get; set; } = new();
        public List<RecentUserVm> RecentUsers { get; set; } = new();
    }

    public class RecentConsumerVm
    {
        public int ConsumerId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime? CreatedAt { get; set; }
    }

    public class RecentUserVm
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    // small summary types (if your controller expects these names)
    public class ConsumerSummaryVm : RecentConsumerVm { }
    public class UserSummaryVm : RecentUserVm { }
}
