namespace Weekly_Consumption_Basics
{
    //Weekly Consumption Basics
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] weeklyConsumption = new int[7];
            
            for (int i = 0; i < weeklyConsumption.Length; i++)
            {
                Console.Write($"Enter consumption for day {i + 1} (kWh): ");
                weeklyConsumption[i] = Convert.ToInt32(Console.ReadLine());
            }
            int Total = weeklyConsumption.Sum();
            double Average = weeklyConsumption.Average();
            int Max = weeklyConsumption.Max();
            int maxDay = Array.IndexOf(weeklyConsumption, Max) + 1;
            int Outages = 0;

            for (int i = 0; i < weeklyConsumption.Length; i++)
            {  
                if (weeklyConsumption[i] == 0)
                {
                    Outages++;
                }
            }

            Console.WriteLine($"Total: {Total} kWh, Average: {Average:F2} kWh, Max: {Max} kWh on Day({maxDay}) | Outages: {Outages}");
        }
    }
}
