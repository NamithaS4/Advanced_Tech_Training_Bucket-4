namespace Quick_Bill_from_Two_Readings
{
    //Quick Bill from Two Readings
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Meter Serial Number:");
            string meterSerial = Console.ReadLine();
            Console.Write("Previous Readings:");
            int prevReading = Convert.ToInt32(Console.ReadLine());
            Console.Write("Current Readings:");
            int currReading = Convert.ToInt32(Console.ReadLine());

            int units = currReading - prevReading;

            if (units <= 0)
            {
                Console.WriteLine("Error");
            }
            else
            {
                double energyCharge = units * 6.5;

                double tax = energyCharge * 0.05;

                double total = energyCharge + tax;

                Console.WriteLine($"Meter: {meterSerial}| Units: {units}| Energy: {energyCharge:F2}| Tax: {tax:F2}| Total Bill: {total:F2}");
            }
        }
    }
}
