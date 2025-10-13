namespace Task_3
{
    //Static Member and Method Practice

        public class Company
        {
            public static string CompanyName = "Esyasoft Technologies Pvt. Ltd.";
            public string EmployeeName { get; set; }
            public int EmployeeId { get; set; }

            public Company(string employeeName, int employeeId)
            {
                EmployeeName = employeeName;
                EmployeeId = employeeId;
            }

            public static void DisplayCompanyName()
            {
                Console.WriteLine($"Company Name: {CompanyName}");
            }

            public void DisplayEmployeeDetails()
            {
                Console.WriteLine($"\n--- Employee Details ---");
                Console.WriteLine($"Employee ID: {EmployeeId}");
                Console.WriteLine($"Name: {EmployeeName}");
                Console.WriteLine($"Company: {CompanyName}");
                Console.WriteLine("------------------------");
            }
        }
        internal class Program
        {
            static void Main(string[] args)
            {
                Company employee1 = new Company("Namii", 101);
                Company employee2 = new Company("Koushii", 102);
                Company employee3 = new Company("Rakesh", 103);

                Console.WriteLine("\nDemonstrating Static and Instance Members\n");

                Console.Write("All employees belong to the same company");
                Company.DisplayCompanyName();

                employee1.DisplayEmployeeDetails();
                employee2.DisplayEmployeeDetails();
                employee3.DisplayEmployeeDetails();
            }
        }
}
