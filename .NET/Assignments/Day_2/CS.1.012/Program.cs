using System.Globalization;
using System.Text;

namespace CS._1._012
{
    public class Tariff
    {
        public string Name { get; private set; }
        public double RatePerKwh { get; private set; }
        public double FixedCharge { get; private set; }

        public Tariff(string name, double rate, double fixedCharge)
        {
            Name = name;
            RatePerKwh = rate;
            FixedCharge = fixedCharge;
            Validate();
        }

        public Tariff(string name, double rate) : this(name, rate, 50.0) { }

        public Tariff(string name) : this(name, 6.0) { }

        public double ComputeBill(int units)
        {
            return units * RatePerKwh + FixedCharge;
        }

        private void Validate()
        {
            if (RatePerKwh <= 0)
            {
                throw new ArgumentException("Rate per kWh must be greater than zero.", nameof(RatePerKwh));
            }
            if (FixedCharge < 0)
            {
                throw new ArgumentException("Fixed charge cannot be negative.", nameof(FixedCharge));
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                var domesticTariff = new Tariff("DOMESTIC");
                var commercialTariff = new Tariff("COMMERCIAL", 9.50);
                var agriculturalTariff = new Tariff("AGRI", 3.0, 50.0);

                int unitsConsumed = 120;

                Console.WriteLine($"DOMESTIC: {domesticTariff.ComputeBill(unitsConsumed).ToString("C", new CultureInfo("en-IN"))}");
                Console.WriteLine($"COMMERCIAL: {commercialTariff.ComputeBill(unitsConsumed).ToString("C", new CultureInfo("en-IN"))}");
                Console.WriteLine($"AGRI: {agriculturalTariff.ComputeBill(unitsConsumed).ToString("C", new CultureInfo("en-IN"))}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}