namespace CS._3._017
{
    public class LoadProfileDay
    {
        public DateTime Date { get; }
        public int[] HourlyKwh { get; }
        public int Total => HourlyKwh.Sum();
        public int PeakHour => Array.IndexOf(HourlyKwh, HourlyKwh.Max());
        public int OutageHours => HourlyKwh.Count(kwh => kwh == 0);

        public LoadProfileDay(DateTime date, int[] hourly)
        {
            if (hourly == null) throw new ArgumentNullException(nameof(hourly));
            if (hourly.Length != 24) throw new ArgumentException("Hourly array must contain exactly 24 values.", nameof(hourly));
            if (hourly.Any(kwh => kwh < 0)) throw new ArgumentException("Hourly kWh values cannot be negative.", nameof(hourly));

            Date = date;
            HourlyKwh = new int[24];
            Array.Copy(hourly, HourlyKwh, 24);
        }
    }

    public abstract class AlarmRule
    {
        public string Name { get; }

        protected AlarmRule(string name)
        {
            Name = name;
        }

        public abstract bool IsTriggered(LoadProfileDay day);

        public virtual string Message(LoadProfileDay day)
        {
            return $"{Name} triggered on {day.Date:yyyy-MM-dd}";
        }
    }
    public class PeakOveruseRule : AlarmRule
    {
        private readonly int _threshold;

        public PeakOveruseRule(int threshold) : base("PeakOveruse")
        {
            _threshold = threshold;
        }

        public override bool IsTriggered(LoadProfileDay day)
        {
            return day.Total > _threshold;
        }
    }
    public class SustainedOutageRule : AlarmRule
    {
        private readonly int _minConsecutive;
        private int _startHourOfOutage = -1;

        public SustainedOutageRule(int min) : base("SustainedOutage")
        {
            _minConsecutive = min;
        }

        public override bool IsTriggered(LoadProfileDay day)
        {
            int consecutiveZeros = 0;
            _startHourOfOutage = -1;

            for (int i = 0; i < day.HourlyKwh.Length; i++)
            {
                if (day.HourlyKwh[i] == 0)
                {
                    if (consecutiveZeros == 0)
                    {
                        _startHourOfOutage = i;
                    }
                    consecutiveZeros++;
                    if (consecutiveZeros >= _minConsecutive)
                    {
                        return true;
                    }
                }
                else
                {
                    consecutiveZeros = 0;
                    _startHourOfOutage = -1;
                }
            }
            return false;
        }
        public override string Message(LoadProfileDay day)
        {
            if (_startHourOfOutage != -1)
            {
                return $"{Name} triggered on {day.Date:yyyy-MM-dd}, starting at hour {_startHourOfOutage}";
            }
            return base.Message(day);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var hourlyData = new int[24];
            for (int i = 0; i < 24; i++)
            {
                if (i >= 10 && i < 14)
                {
                    hourlyData[i] = 0;
                }
                else if (i == 19)
                {
                    hourlyData[i] = 15;
                }
                else
                {
                    hourlyData[i] = 2;
                }
            }

            var day = new LoadProfileDay(new DateTime(2025, 10, 1), hourlyData);

            var rules = new List<AlarmRule>
            {
                new PeakOveruseRule(threshold: 30),
                new SustainedOutageRule(min: 3)
            };

            foreach (var rule in rules)
            {
                if (rule.IsTriggered(day))
                {
                    Console.WriteLine(rule.Message(day));
                }
            }
        }
    }
}