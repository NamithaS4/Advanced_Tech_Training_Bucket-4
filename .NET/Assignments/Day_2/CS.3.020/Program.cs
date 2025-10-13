namespace CS._3._020
{
    public abstract class Event : IComparable<Event>
    {
        public DateTime When { get; }
        public string MeterSerial { get; }

        protected Event(DateTime when, string meterSerial)
        {
            When = when;
            MeterSerial = meterSerial;
        }

        public abstract string Category { get; }
        public abstract int Severity { get; }

        public virtual string Describe() =>
            $"{When:yyyy-MM-dd HH:mm} [{Category}] {MeterSerial}";

        public int CompareTo(Event? other)
        {
            if (other == null) return -1;
            int severityComparison = other.Severity.CompareTo(this.Severity);
            if (severityComparison != 0)
                return severityComparison;
            return other.When.CompareTo(this.When);
        }
    }


    public class OutageEvent : Event
    {
        public int DurationMinutes { get; }

        public OutageEvent(DateTime when, string meterSerial, int durationMinutes)
            : base(when, meterSerial)
        {
            DurationMinutes = durationMinutes;
        }

        public override string Category => "OUTAGE";
        public override int Severity => 3;

        public override string Describe() =>
            $"{base.Describe()} | Duration: {DurationMinutes} min | Severity: {Severity}";
    }

    public class TamperEvent : Event
    {
        public string Code { get; }

        public TamperEvent(DateTime when, string meterSerial, string code)
            : base(when, meterSerial)
        {
            Code = code;
        }

        public override string Category => "TAMPER";
        public override int Severity => 5;

        public override string Describe() =>
            $"{base.Describe()} | Code: {Code} | Severity: {Severity}";
    }

    public class VoltageEvent : Event
    {
        public int Voltage { get; }

        public VoltageEvent(DateTime when, string meterSerial, int voltage)
            : base(when, meterSerial)
        {
            Voltage = voltage;
        }

        public override string Category => "VOLTAGE";
        public override int Severity => 2;

        public override string Describe() =>
            $"{base.Describe()} | V: {Voltage} | Severity: {Severity}";
    }

    public class EventProcessor
    {
        public static void PrintTopSevere(IEnumerable<Event> events, int topN)
        {
            var sorted = events
                .OrderByDescending(e => e.Severity)
                .ThenByDescending(e => e.When)
                .Take(topN);

            foreach (var e in sorted)
            {
                Console.WriteLine(e.Describe());
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var events = new List<Event>
            {
                new OutageEvent(new DateTime(2025,10,05,22,10,0), "AP-0003", 95),
                new TamperEvent(new DateTime(2025,10,06,09,20,0), "AP-0007", "MISMATCH"),
                new VoltageEvent(new DateTime(2025,10,05,18,0,0), "AP-0001", 184),
                new OutageEvent(new DateTime(2025,10,04,17,0,0), "AP-0002", 45),
                new TamperEvent(new DateTime(2025,10,02,11,30,0), "AP-0005", "COVER_OPEN"),
                new VoltageEvent(new DateTime(2025,10,03,09,0,0), "AP-0004", 192),
                new OutageEvent(new DateTime(2025,10,06,08,30,0), "AP-0006", 30),
                new VoltageEvent(new DateTime(2025,10,05,10,0,0), "AP-0008", 177)
            };

            Console.WriteLine("Top 3 Severe Events:");
            EventProcessor.PrintTopSevere(events, 3);

            Console.WriteLine("Sorting via IComparable:");
            events.Sort();
            foreach (var e in events.Take(3))
            {
                Console.WriteLine(e.Describe());

            }
        }
    }
}