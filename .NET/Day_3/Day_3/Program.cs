namespace Day_3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> food = new List<string>();
            
                food.Add("Pizza");
                food.Add("Burger");
                food.Add("Pasta");
                food.Add("Sushi");

            food.Remove("Burger");
            food.Sort();
            food.Insert(1, "Tacos");
            Console.WriteLine(food.Count);

        }
    }
}
