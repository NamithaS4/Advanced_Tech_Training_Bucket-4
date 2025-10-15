namespace CS._3._005
{
    //Monthly Slab Billing with Category & Outage Check
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int[] pattern = { 4, 4, 5, 5, 0, 6, 7, 3, 4, 5 };
            string category = "COMMERCIAL";

            //Builds full 30-day month by repeating the pattern 3 times
            int[] month = new int[30];
            for (int i = 0; i < 30; i++)
            {
                month[i] = pattern[i % 10];
            }

            //Computes monthly total and outage count
            int monthlyUnits = 0;
            int outageDays = 0;
            foreach (int units in month)
            {
                monthlyUnits += units;
                if (units == 0)
                    outageDays++;
            }

            //Computes energy charge using slabs
            double energyCharge = 0;
            int remainingUnits = monthlyUnits;

            if (remainingUnits > 0)
            {
                if (remainingUnits <= 100)
                {
                    energyCharge = remainingUnits * 4.0;
                }
                else if (remainingUnits <= 300)
                {
                    energyCharge = (100 * 4.0) + ((remainingUnits - 100) * 6.0);
                }
                else
                {
                    energyCharge = (100 * 4.0) + (200 * 6.0) + ((remainingUnits - 300) * 8.5);
                }
            }

            //Fixed charge based on category
            double fixedCharge = 0;
            switch (category.ToUpper())
            {
                case "DOMESTIC":
                    fixedCharge = 50.0;
                    break;
                case "COMMERCIAL":
                    fixedCharge = 150.0;
                    break;
                default:
                    Console.WriteLine("Invalid category!");
                    return;
            }

            //Checks outage condition and apply rebate if no outages
            double rebate = 0;
            double subtotal = energyCharge + fixedCharge;
            if (outageDays == 0)
            {
                rebate = subtotal * 0.02;
            }

            double total = subtotal - rebate;

            //Prints the result neatly
            Console.WriteLine($"Category: {category} | Units: {monthlyUnits} | Energy: ₹{energyCharge:F2} | Fixed: ₹{fixedCharge:F2} | Rebate: ₹{rebate:F2} | Total: ₹{total:F2} | Outages: {outageDays}");

            // Stretch Goals
            Console.WriteLine("\n Slab Breakdown: ");
            if (monthlyUnits > 0)
            {
                if (monthlyUnits <= 100)
                {
                    Console.WriteLine($"First {monthlyUnits} @ ₹4.0 = ₹{monthlyUnits * 4.0:F2}");
                }
                else if (monthlyUnits <= 300)
                {
                    Console.WriteLine("First 100 @ ₹4.0 = ₹400.00");
                    Console.WriteLine($"Next {monthlyUnits - 100} @ ₹6.0 = ₹{(monthlyUnits - 100) * 6.0:F2}");
                }
                else
                {
                    Console.WriteLine("First 100 @ ₹4.0 = ₹400.00");
                    Console.WriteLine("Next 200 @ ₹6.0 = ₹1200.00");
                    Console.WriteLine($"Remaining {monthlyUnits - 300} @ ₹8.5 = ₹{(monthlyUnits - 300) * 8.5:F2}");
                }
            }
        }
    }
}
