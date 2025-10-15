namespace CS._3._010
{
    //Multi-meter Outage Duration Analyzer
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] meters = { "MTR001", "MTR002", "MTR003" };
            int[,] outageHours =
            {
            { 1, 0, 0, 5, 2, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0 },
            { 0, 4, 3, 0, 0, 0, 0 }
        };

            //Stretch goals
            int i = 0;
            bool invalid = false;
            while (i < meters.Length && !invalid)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (outageHours[i, j] < 0)
                    {
                        invalid = true;
                        break;
                    }
                }
                i++;
            }

            if (invalid)
            {
                Console.WriteLine("Invalid Data detected!");
                return;
            }

            
            for (int m = 0; m < meters.Length; m++)
            {
                int total = 0;

                for (int d = 0; d < 7; d++)
                {
                    total += outageHours[m, d];
                }

                string action;
                if (total > 8)
                    action = "Escalate to field team";
                else if (total == 0)
                    action = "Stable";
                else
                    action = "Monitor";

                Console.WriteLine($"{meters[m]} | Outage Hours: {total} | Action: {action}");
            }
        }
    }
}
