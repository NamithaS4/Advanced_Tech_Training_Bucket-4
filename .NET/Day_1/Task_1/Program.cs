namespace Task_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string studentName = Console.ReadLine();

            int mathematics = Convert.ToInt32(Console.ReadLine());
            int physics = Convert.ToInt32(Console.ReadLine());
            int social = Convert.ToInt32(Console.ReadLine());
            int politics = Convert.ToInt32(Console.ReadLine());
            int foreignLanguage = Convert.ToInt32(Console.ReadLine());

            int totalMarks = mathematics + physics + social + politics + foreignLanguage;
            double averageMarks = totalMarks / 5.0;
            double percentage = (totalMarks / 500.0) * 100;

            Console.WriteLine($"Student Name: {studentName}");
            Console.WriteLine("Results:");
            Console.WriteLine($"Total Marks: {totalMarks}");
            Console.WriteLine($"Average Marks: {averageMarks}");
            Console.WriteLine($"Percentage: {percentage}%");

            Console.ReadLine();

        }
    }
}
