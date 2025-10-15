namespace CS._2._008
{
    //Loop-based Daily Consumption Summary
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            double[] daily = { 5.2, 6.8, 0.0, 7.5, 6.0, 4.8, 0.0 };

            double total = 0;
            int peakDays = 0;
            int outageDays = 0;

            for (int i = 0; i < daily.Length; i++)
            {
                total += daily[i];

                if (daily[i] > 6)
                    peakDays++;

                if (daily[i] == 0)
                    outageDays++;
            }

            double average = total / daily.Length;

            Console.WriteLine($"Total: {total:F1} kWh | Avg: {average:F2} kWh | Peak Days (>6): {peakDays} | Outages: {outageDays}");

            // Stretch Goals
            string status = (outageDays <= 1 && peakDays <= 2) ? "Stable" : "Unstable";
            Console.WriteLine($"Performance Status: {status}");
        }
    
    }
}
