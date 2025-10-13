namespace CS._1._011
{
        public class Meter
        {
            public required string MeterSerial { get; init; }
            public required string Location { get; init; }
            public DateTime InstalledOn { get; init; } = DateTime.Today;

            private int _lastReadingKwh;
            public int LastReadingKwh
            {
                get => _lastReadingKwh;
                init
                {
                    if (value >= 0)
                    {
                        _lastReadingKwh = value;
                    }
                }
            }

            public void AddReading(int deltaKwh)
            {
                if (deltaKwh > 0)
                {
                    _lastReadingKwh += deltaKwh;
                    Console.WriteLine($"Added {deltaKwh} deltaKwh to Reading!\n");
                }
                else
                {
                    Console.WriteLine("Invalid reading. Value must be positive.\n");
                }
            }

            public string Summary(int serialWidth, int locationWidth, int readingWidth)
            {
                return string.Format("{0,-" + serialWidth + "} Location: {1,-" + locationWidth + "} | Reading: {2," + readingWidth + "}",
                    MeterSerial, Location, LastReadingKwh);
            }

            public override string ToString()
            {
                return $"{MeterSerial} Location: {Location} | Reading: {LastReadingKwh}";
            }
        }

        internal class Program
        {
            static void Main(string[] args)
            {
                var meter1 = new Meter
                {
                    MeterSerial = "AP-0001",
                    Location = "Feeder-12",
                    LastReadingKwh = 15000
                };

                var meter2 = new Meter
                {
                    MeterSerial = "AP-0002",
                    Location = "DTR-9",
                    LastReadingKwh = 9500
                };

               
                meter1.AddReading(230);
                meter2.AddReading(300);

                meter1.AddReading(-50);
                meter2.AddReading(0);

                
                int maxSerialWidth = Math.Max(meter1.MeterSerial.Length, meter2.MeterSerial.Length);
                int maxLocationWidth = Math.Max(meter1.Location.Length, meter2.Location.Length);
                int maxReadingWidth = Math.Max(
                    meter1.LastReadingKwh.ToString().Length,
                    meter2.LastReadingKwh.ToString().Length
                );

                Console.WriteLine("Meter Details:");
                Console.WriteLine(meter1.Summary(maxSerialWidth, maxLocationWidth, maxReadingWidth));
                Console.WriteLine(meter2.Summary(maxSerialWidth, maxLocationWidth, maxReadingWidth));

                Console.WriteLine();
            }
        }
}
