namespace CS._3._004
{
    //Multi-Meter Weekly Health Report
    internal class Program
    {
        static void Main(string[] args)
        {
            int[][] meter = new int[][]
            {
                new[] { 4, 5, 0, 0, 6, 7, 3 },
                new[] { 2, 2, 2, 2, 2, 2, 2 },
                new[] { 9, 1, 1, 1, 1, 1, 1 }
            };
            string[] ids= { "A-1001", "B-2001", "C-3001" };

            int globalMax = int.MinValue;
            int globalMeterIndex = -1;
            int globalDayIndex = -1;

            Console.WriteLine("Weekly Health Report");

            for (int m = 0; m < meter.Length; m++)
            {
                int[] days = meter[m];
                string meterId = ids[m];

                int total = 0;
                bool peakAlert = false;
                bool sustainedOutage = false;
                bool underutilization = false;

                int consercutiveZeros = 0;

                for(int d = 0; d < days.Length; d++)
                {
                    int kWh = days[d];
                    total += kWh;

                    if(kWh > 8)
                    {
                        peakAlert = true;
                    }

                    if(kWh == 0)
                    {
                        consercutiveZeros++;
                    }
                    else
                    {
                        consercutiveZeros = 0;
                    }
                    if(consercutiveZeros >= 2)
                    {
                        sustainedOutage = true;
                    }

                    if(kWh > globalMax)
                    {
                        globalMax = kWh;
                        globalMeterIndex = m;
                        globalDayIndex = d;
                    }
                }
                double average = total / (double)days.Length;

                underutilization = average < 3;


            }
        }
    }
}
