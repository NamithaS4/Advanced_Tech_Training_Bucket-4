namespace Task_4
{
        public class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal BasicSalary { get; set; }
            public decimal Hra { get; private set; }
            public decimal Da { get; private set; }
            public decimal GrossSalary { get; private set; }

            public Employee(int id, string name, decimal basicSalary)
            {
                Id = id;
                Name = name;
                BasicSalary = basicSalary;
            }

            public void CalculateSalary()
            {
                Hra = 0.10m * BasicSalary;
                Da = 0.05m * BasicSalary;
                GrossSalary = BasicSalary + Hra + Da;
            }

            public void DisplaySalarySlip()
            {
                

                Console.WriteLine("\n=========================");
                Console.WriteLine("     SALARY SLIP");
                Console.WriteLine("=========================");
                Console.WriteLine($"Employee ID: {Id}");
                Console.WriteLine($"Employee Name: {Name}");
                Console.WriteLine("-------------------------");
                Console.WriteLine($"Basic Salary: Rs {BasicSalary}");
                Console.WriteLine($"HRA (10%): Rs {Hra}");
                Console.WriteLine($"DA (5%): Rs {Da}");
                Console.WriteLine("-------------------------");
                Console.WriteLine($"Gross Salary: Rs {GrossSalary}");
                Console.WriteLine("=========================\n");
            }
        }
        internal class Program
        {
            static void Main(string[] args)
            {
                Employee employee1 = new Employee(101, "Namii", 5000000);
                Employee employee2 = new Employee(102, "Koushii", 7500000);

                employee1.CalculateSalary();
                employee2.CalculateSalary();

                employee1.DisplaySalarySlip();
                employee2.DisplaySalarySlip();
            }
        }
}
