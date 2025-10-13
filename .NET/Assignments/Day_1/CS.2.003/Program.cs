namespace CS._2._003
{
        internal class Program
        {
            static void Main(string[] args)
            {
                

                string[] lines = {
                "2025-09-01,4.2,OK",
                "2025-09-02,5.0,OK",
                "2025-09-03,0.0,OUTAGE",
                "2025-09-04,3.8,OK",
                "2025-09-05,6.1,OK",
                "2025-09-06,2.5,TAMPER",
                "2025-09-07,5.4,OK"
            };

                
                double okSum = 0;
                int okCount = 0;
                int outageCount = 0;
                int tamperCount = 0;

                double maxOk = 0;
                string busiestDay = "";

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');

                    if (parts.Length != 3)
                    {
                        Console.WriteLine($"Skipping invalid line: {line}");
                        continue;
                    }

                    string date = parts[0];
                    double kWh;
                    string status = parts[2].Trim().ToUpper();

                    
                    if (!double.TryParse(parts[1], out kWh))
                    {
                        Console.WriteLine($"Invalid kWh value in line: {line}");
                        continue;
                    }

                    
                    if (status == "OK")
                    {
                        okSum += kWh;
                        okCount++;

                        if (kWh > maxOk)
                        {
                            maxOk = kWh;
                            busiestDay = date;
                        }
                    }
                    else if (status == "OUTAGE")
                    {
                        outageCount++;
                    }
                    else if (status == "TAMPER")
                    {
                        tamperCount++;
                    }
                }

                
                double okAvg = okCount > 0 ? okSum / okCount : 0;

                
                Console.WriteLine($"OK: {okSum:F2} kWh (avg {okAvg:F2}) | OUTAGE: {outageCount} | TAMPER: {tamperCount}");
                Console.WriteLine($"Busiest OK day: {busiestDay} ({maxOk:F2} kWh)");
            }
        }
}


