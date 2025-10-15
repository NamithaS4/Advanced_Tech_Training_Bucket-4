namespace CS._3._009
{
    //Analyze Tamper & Outage Patterns
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] status = { "OK", "OUTAGE", "OK", "TAMPER", "OUTAGE", "OK", "LOW_VOLT" };

            int okCount = 0, outageCount = 0, tamperCount = 0, lowVoltCount = 0;
            bool suspicious = false;

            
            for (int i = 0; i < status.Length; i++)
            {
                switch (status[i])
                {
                    case "OK":
                        okCount++;
                        break;
                    case "OUTAGE":
                        outageCount++;
                        break;
                    case "TAMPER":
                        tamperCount++;
                        if (i > 0 && status[i - 1] == "OUTAGE")
                            suspicious = true;
                        break;
                    case "LOW_VOLT":
                        lowVoltCount++;
                        break;
                }
            }

            
            Console.Write($"OK: {okCount} | OUTAGE: {outageCount} | TAMPER: {tamperCount} | LOW_VOLT: {lowVoltCount}  ");

           
            if (outageCount > 2 || tamperCount > 1)
                Console.Write("Maintenance required");
            else
                Console.Write("Meter healthy");

            if (suspicious)
                Console.Write(" | Suspicious Pattern detected!");

            Console.WriteLine();
        }
    }
}
