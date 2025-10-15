namespace CS._2._007
{
    //Meter Category-based Tariff using switch
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; 

            Console.Write("Enter Meter Category (DOMESTIC / COMMERCIAL / AGRICULTURE): ");
            string meterCategory = Console.ReadLine().ToUpper();

            Console.Write("Enter Units Consumed: ");
            double units = Convert.ToDouble(Console.ReadLine());

            
            double rate = 0;
            bool validCategory = true;

            
            switch (meterCategory)
            {
                case "DOMESTIC":
                    rate = 6.0;
                    break;

                case "COMMERCIAL":
                    rate = 8.5;
                    break;

                case "AGRICULTURE":
                    rate = 3.0;
                    break;

                default:
                    Console.WriteLine("Unknown category. Check configuration.");
                    validCategory = false;
                    break;
            }

            
            if (validCategory)
            {
                double totalBill = units * rate;
                Console.WriteLine($"Category: {meterCategory} | Rate: ₹{rate:F2} | Total Bill: ₹{totalBill:F2}");
            }
        }
    }
}
