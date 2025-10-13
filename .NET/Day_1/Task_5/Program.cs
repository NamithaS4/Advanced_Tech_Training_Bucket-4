namespace Task_5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Task1
            Console.WriteLine("Task-1: No. Sq Cube");
            for (int i = 1; i <= 10; i++)
            {
                int square = i * i;
                int cube = i * i * i;

                Console.WriteLine($"{i} {square} {cube}");

            }


            //Task2

            Console.WriteLine("Task-2: The Number of perfect squares in between 1 to 1000");
            for (int n = 1; n <= 1000; n++)
            {
                int sum = 0;

                for (int i = 1; i < n; i++)
                {
                    if (n % i == 0)
                        sum += i;
                }

                if (sum == n)
                    Console.WriteLine(n);
            }

            //Task3

            Console.WriteLine("Task-3: Hourglass pattern");
            int k = 3; 

            for (int i = k; i >= 1; i--)
            {
                for (int s = 0; s < k - i; s++)
                    Console.Write(" ");
                for (int j = 1; j <= (2 * i - 1); j++)
                    Console.Write("*");
                Console.WriteLine();
            }

            for (int i = 2; i <= k; i++)
            {
                for (int s = 0; s < k - i; s++)
                    Console.Write(" ");
                for (int j = 1; j <= (2 * i - 1); j++)
                    Console.Write("*");
                Console.WriteLine();
            }

            //Task4

            Console.WriteLine("Task-4: Diamond of numbers");

            int o = 5;

            for (int i = 1; i <= o; i++)
            {

                for (int s = i; s < o; s++)
                { 
                Console.Write(" ");
                }
                for (int j = 1; j <= i; j++)
                {
                    Console.Write(j);
                }
                for (int j = i - 1; j >= 1; j--)
                {
                    Console.Write(j);
                }

                Console.WriteLine();
            }

            //Task5

            Console.WriteLine("Task-5: Alternate Numbers");

            int l = 5;
                for (int i = 1; i <= l; i++)
                {
                    for (int j = 1; j <= i; j++)
                    {
                        if ((i + j) % 2 == 0)
                        {
                            Console.Write("1 ");
                        }
                        else
                        { 
                            Console.Write("0 ");
                        }
                    }
                    Console.WriteLine();
                }

            //Task6


            Console.WriteLine("Task-6: Armstrong numbers between 100 and 999");

            int start = 100, end = 999;
            for (int num = start; num <= end; num++)
            {
                int sum = 0;
                int temp = num;
                while (temp != 0)
                {
                    int digit = temp % 10;
                    sum += digit * digit * digit;
                    temp /= 10;
                }
                if (sum == num)
                {
                    Console.WriteLine(num);
                }
            }

            //Task7

            Console.WriteLine("Task-7: Fibonacci series in reverse order");

            int t = Convert.ToInt32(Console.ReadLine());
            int first = 0, second = 1;      
            int[] fibonacci = new int[t];
            for (int i = 0; i < t; i++)
            {
                fibonacci[i] = first;
                int next = first + second;
                first = second;
                second = next;
            }
            for (int i = t - 1; i >= 0; i--)
            {
                Console.Write(fibonacci[i] + " ");
            }
            Console.WriteLine();

            //Task8

            Console.WriteLine("Task-8: Zigzag star pattern");

            int height = 4;
            for (int i = 1; i <= height; i++)
            {
                for (int j = 1; j <= height; j++)
                {
                    for (int s = i; s < height; s++)
                    {
                        Console.Write(" ");
                    }
                    if ((i + j) % 2 == 0)
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }

            //Task9

            Console.WriteLine("Task-9: Total digits in a number using a loop");

            int number = Convert.ToInt32(Console.ReadLine());
            int count = 0;
            while (number != 0)
            {
                number /= 10;
                count++;
            }
            Console.WriteLine("Total digits: " + count);

            //Task10

            Console.WriteLine("Task-10: Diamond pattern with numbers");

            int p = 5;

            for (int i = 1; i <= p; i++)
            {

                for (int s = i; s < p; s++)
                {
                    Console.Write(" ");
                }
                for (int j = 1; j <= i; j++)
                {
                    Console.Write(j);
                }
                for (int j = i - 1; j >= 1; j--)
                {
                    Console.Write(j);
                }

                Console.WriteLine();
            }
            for (int i = p - 1; i >= 1; i--)
            {
                for (int s = i; s < p; s++)
                {
                    Console.Write(" ");
                }
                for (int j = 1; j <= i; j++)
                {
                    Console.Write(j);
                }
                for (int j = i - 1; j >= 1; j--)
                {
                    Console.Write(j);
                }
                Console.WriteLine();
            }


            Console.ReadLine();
        }
    }
}
