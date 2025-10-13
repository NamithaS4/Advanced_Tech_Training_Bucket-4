namespace Day_2
{
    public class BankAccount
    {
        private decimal balance;
        public decimal Balance
        {
            get { return balance; }
            private set { balance = value; }
        }
        
        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                balance += amount;
                Console.WriteLine($"The Amount Deposited: {amount}\nNew Balance: {balance}\n");
            }
            else
            {
                Console.WriteLine("Deposit amount must be positive.\n");
            }
        }
        public void Withdraw(decimal amount)
        {
            if (amount > 0 && amount <= balance)
            {
                balance -= amount;

                Console.WriteLine($"The amount Withdrew: {amount}\nNew Balance: {balance}\n");
            }
            else
            {
                Console.WriteLine("Invalid withdrawal amount.\n");
            }
        }
        public decimal GetBalance()
        {
            return balance;
        }
    }
    //static class Message
    //{
    //    public static void Show()
    //    {
    //        Console.WriteLine("Hello, Namiiii!");
    //        Console.WriteLine("Good Morning");
    //    }
    //}

    //class Animal
    //{
    //    public static void Speak()
    //    {
    //        Console.WriteLine("The animal makes a sound");
    //    }
    //}

    //class Dog : Animal
    //{
    //    public static void Speak()
    //    {
    //        Console.WriteLine("The dog barks");
    //    }
    //}
    //abstract class vehicle
    //{
    //    public int speed = 0;
    //    public void go()
    //    {
    //        Console.WriteLine("The vehicle is moving");
    //    }
    //}
    //class car : vehicle
    //{
    //    int wheels = 4;

    //}
    //public class organs
    //{
    //    public string head;
    //    public string heart;
    //}
    //class human : organs
    //{
    //    public string name;
    //    public int age;

    //    public human(string name, int age)
    //    {
    //        this.name = name;
    //        this.age = age;
    //    }
    //    public void eat()
    //    {
    //        Console.WriteLine($"{name} is eating IceCreammmm. which makes her {head} feel better.");
    //    }
    //    public void sleep()
    //    {
    //        Console.WriteLine($"{name} is sleeping peacefully and her heart is {heart}.");
    //    }

    //}
    internal class Program
    {
        static void Main(string[] args)
        {
            //Message.Show();
            //car myCar = new car();
            //myCar.go();
          
            //Dog dog = new Dog();
            //Dog.Speak();
            //human person1 = new human("Namiii", 20);

            //person1.head = "Brain";
            //person1.heart = "Pumping Blood";

            //person1.eat();
            //person1.sleep();
            //Console.WriteLine($"Name: {person1.name}, Age: {person1.age}");

            BankAccount account = new BankAccount();

            account.Deposit(500);
            account.Withdraw(200);
            account.Withdraw(400); 
            account.Deposit(5000);

            Console.WriteLine($"Final Balance: {account.GetBalance()}");

            Console.ReadLine();
        }
    }
}
