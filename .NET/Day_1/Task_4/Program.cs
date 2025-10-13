namespace Task_4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int totalMinutes = Convert.ToInt32(Console.ReadLine());
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;

            Console.WriteLine("Total Minutes to Hours and minutes:");
            Console.WriteLine($"{hours} {minutes}");
            Console.ReadLine();
        }
    }
}
