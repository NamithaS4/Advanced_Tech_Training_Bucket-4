namespace CS._1._006
{
    //Compute Net Consumption and Alerts
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; 

            
            Console.Write("Enter Previous Reading (kWh): ");
            double previous = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Current Reading (kWh): ");
            double current = Convert.ToDouble(Console.ReadLine());

            
            double consumption = current - previous;

            
            if (consumption < 0)
            {
                Console.WriteLine("Invalid readings! (Current < Previous)");
            }
            else if (consumption == 0)
            {
                Console.WriteLine("Net Consumption: 0 kWh | Possible outage.");
            }
            else
            {
                Console.Write($"Net Consumption: {consumption} kWh");

                if (consumption > 500)
                {
                    Console.WriteLine(" | High Consumption Alert!");
                }
                else if (consumption > 100 && consumption < 500)
                {
                    Console.WriteLine(" | Normal usage.");
                }
                else
                {
                    Console.WriteLine(" | Low consumption.");
                }
            }
        }
    }
}
