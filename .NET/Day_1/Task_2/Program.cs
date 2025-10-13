namespace Task_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double BasicSalary = Convert.ToDouble(Console.ReadLine());
            double HRA = 0.20 * BasicSalary;
            double DA = 0.10 * BasicSalary;
            double tax = 0.08 * BasicSalary;

            double grossSalary = BasicSalary + HRA + DA;
            double netSalary = grossSalary - tax;

            Console.WriteLine("Salary Details:");
            Console.WriteLine($"Basic Salary: {BasicSalary}");
            Console.WriteLine($"Hosue Rent Allowance (20% of BS): {HRA}");
            Console.WriteLine($"Dearness Allowance (10% of BS): {DA}");
            Console.WriteLine($"Tax Deducted (8% of BS): {tax}");
            Console.WriteLine($"Gross Salary: {grossSalary}");
            Console.WriteLine($"Net Salary: {netSalary}");

            Console.ReadLine();
        }
    }
}
