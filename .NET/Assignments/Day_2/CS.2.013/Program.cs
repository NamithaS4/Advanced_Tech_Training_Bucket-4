namespace CS._2._013
{
    public class Device
    {
        public string Id { get; init; }
        public DateTime InstalledOn { get; init; }

        protected Device(string id, DateTime installedOn)
        {
            Id = id;
            InstalledOn = installedOn;
        }
        public virtual string Describe()
        {
            return $"Device Id: {Id} | Installed: {InstalledOn:yyyy-MM-dd}";
        }
    }

    public class Meter : Device
    {
        public int PhaseCount { get; init; }

        public Meter(string id, DateTime installedOn, int phaseCount) : base(id, installedOn)
        {
            PhaseCount = phaseCount;
        }

        public override string Describe()
        {
            return $"{base.Describe()} | Phases: {PhaseCount}";
        }
    }
    public class Gateway : Device
    {
        public string IpAddress { get; init; }

        public Gateway(string id, DateTime installedOn, string ipAddress) : base(id, installedOn)
        {
            IpAddress = ipAddress;
        }

        public override string Describe()
        {
            return $"{base.Describe()} | IP: {IpAddress}";
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var devices = new List<Device>
            {
                new Meter("AP-0001", new DateTime(2024, 7, 1), 3),
                new Gateway("GW-11", new DateTime(2025, 1, 10), "10.0.5.21")
            };

            int maxHeaderWidth = devices.Max(d => d.GetType().Name.Length);
            int maxIdWidth = devices.Max(d => d.Id.Length);
            int maxUniqueDescWidth = devices.Max(d => d.Describe().Length - d.Describe().IndexOf('|') - 2);

            foreach (var d in devices)
            {
                string baseDescription = d.Describe();
                string fullDescription = d is Meter m ? $"{m.Describe()} | Phases: {m.PhaseCount}" : d is Gateway g ? $"{g.Describe()} | IP: {g.IpAddress}" : baseDescription;

                var parts = fullDescription.Split(new[] { " | " }, StringSplitOptions.None);

                Console.WriteLine(string.Format(
                    $"{{0,-{maxHeaderWidth}}} Id: {{1,-{maxIdWidth}}} | Installed: {{2,-10}} | {{3,-{maxUniqueDescWidth}}}",
                    d.GetType().Name,
                    d.Id,
                    d.InstalledOn.ToString("yyyy-MM-dd"),
                    parts.Last()
                ));
            }
        }
    }
}