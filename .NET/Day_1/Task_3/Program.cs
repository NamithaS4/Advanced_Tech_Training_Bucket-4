namespace Task_3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double INR = Convert.ToDouble(Console.ReadLine());
            double USD = Math.Round(INR / 83.0, 2);
            double EUR = Math.Round(INR / 90.5, 2);

            Console.WriteLine("Currency Conversion");
            Console.WriteLine($"In INR: {INR} ");
            Console.WriteLine($"In USD: {USD}");
            Console.WriteLine($"In EUR: {EUR}");

            Console.ReadLine();
        }
    }
}
