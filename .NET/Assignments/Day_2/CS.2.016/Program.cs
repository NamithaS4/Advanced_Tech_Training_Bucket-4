namespace CS._2._016
{
    public class LoadProfileDay
    {
        public DateTime Date { get; }
        public int[] HourlyKwh { get; }
        public int Total => HourlyKwh.Sum();
        public int PeakHour
        {
            get
            {
                int maxKwh = HourlyKwh.Max();
                return Array.IndexOf(HourlyKwh, maxKwh);
            }
        }
        public int OutageHours => HourlyKwh.Count(kwh => kwh == 0);

        public LoadProfileDay(DateTime date, int[] hourly)
        {
            if (hourly == null)
            {
                throw new ArgumentNullException(nameof(hourly));
            }
            if (hourly.Length != 24)
            {
                throw new ArgumentException("Hourly array must contain exactly 24 values.", nameof(hourly));
            }
            if (hourly.Any(kwh => kwh < 0))
            {
                throw new ArgumentException("Hourly kWh values cannot be negative.", nameof(hourly));
            }

            Date = date;

            HourlyKwh = new int[24];
            Array.Copy(hourly, HourlyKwh, 24);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var hourlyData = new int[24];
            for (int i = 0; i < 24; i++)
            {
                if (i == 19)
                {
                    hourlyData[i] = 10;
                }
                else if (i == 0 || i == 1)
                {
                    hourlyData[i] = 0;
                }
                else
                {
                    hourlyData[i] = 3;
                }
            }

            var today = new DateTime(2025, 10, 1);

            var profile = new LoadProfileDay(today, hourlyData);

            Console.WriteLine($"{profile.Date:yyyy-MM-dd} | Total: {profile.Total} kWh | PeakHour: {profile.PeakHour}");

            Console.WriteLine($"Outage Hours: {profile.OutageHours}");

            hourlyData[0] = 999;
            Console.WriteLine($"\nAfter modifying original array, the object's total is still: {profile.Total}");
        }
    }
}