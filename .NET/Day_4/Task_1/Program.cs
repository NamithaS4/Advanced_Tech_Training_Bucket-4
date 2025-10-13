
using Task_1;

var employees = new List<Employee>
{
    new Employee{ Id=1, Name="Ravi", Department="IT", Salary=85000, Experience=5, Location="Bangalore"},
    new Employee{ Id=2, Name="Priya", Department="HR", Salary=52000, Experience=4, Location="Pune"},
    new Employee{ Id=3, Name="Kiran", Department="Finance", Salary=73000, Experience=6, Location="Hyderabad"},
    new Employee{ Id=4, Name="Asha", Department="IT", Salary=95000, Experience=8, Location="Bangalore"},
    new Employee{ Id=5, Name="Vijay", Department="Marketing", Salary=68000, Experience=5, Location="Mumbai"},
    new Employee{ Id=6, Name="Deepa", Department="HR", Salary=61000, Experience=7, Location="Delhi"},
    new Employee{ Id=7, Name="Arjun", Department="Finance", Salary=82000, Experience=9, Location="Bangalore"},
    new Employee{ Id=8, Name="Sneha", Department="IT", Salary=78000, Experience=4, Location="Pune"},
    new Employee{ Id=9, Name="Rohit", Department="Marketing", Salary=90000, Experience=10, Location="Delhi"},
    new Employee{ Id=10, Name="Meena", Department="Finance", Salary=66000, Experience=3, Location="Mumbai"}
};

//Display all employees working in the IT.
var itEmployees = employees.Where(e => e.Department == "IT");
Console.WriteLine("1. Employees Working in IT department:");
foreach (var e in itEmployees)
    Console.WriteLine($"- {e.Name}");

//List of names and salaries of employees who earn more than 70,000.
var highPaidEmployees = employees.Where(e => e.Salary > 70000);
Console.WriteLine("\n2. Employees earning more than 70,000:");
foreach (var e in highPaidEmployees)
    Console.WriteLine($"- {e.Name}: INR {e.Salary}");

//Finds all employees located in Bangalore.
var bangaloreEmployees = employees.Where(e => e.Location == "Bangalore");
Console.WriteLine("\n3. Employees Located in Bangalore:");
foreach (var e in bangaloreEmployees)
    Console.WriteLine($"- {e.Name}");

// 4. Display employees having more than 5 years of experience.
var experiencedEmployees = employees.Where(e => e.Experience > 5);
Console.WriteLine("\n4. Employees with more than 5 years of experience:");
foreach (var e in experiencedEmployees)
    Console.WriteLine($"- {e.Name}");

//Shows names and salaries in ascending order of salary.
var sortedBySalary = employees.OrderBy(e => e.Salary);
Console.WriteLine("\n5. List of Employees sorted by salary (ascending):");
foreach (var e in sortedBySalary)
    Console.WriteLine($"- {e.Name}: INR {e.Salary}");

//Group employees by location and count how many employees are in each location.
var employeesByLocation = employees.GroupBy(e => e.Location)
                                    .Select(g => new { Location = g.Key, Count = g.Count() });
Console.WriteLine("\n6. Employee count by location:");
foreach (var group in employeesByLocation)
    Console.WriteLine($"- {group.Location}: {group.Count}");

//Display employees whose salary is above the average salary.
var averageSalary = employees.Average(e => e.Salary);
var aboveAverageSalary = employees.Where(e => e.Salary > averageSalary);
Console.WriteLine($"\n7. Employees with salary above the average (INR {averageSalary}):");
foreach (var e in aboveAverageSalary)
    Console.WriteLine($"- {e.Name}");

//Show top 3 highest-paid employees.
var top3Paid = employees.OrderByDescending(e => e.Salary).Take(3);
Console.WriteLine("\n8. Top 3 highest-paid employees:");
foreach (var e in top3Paid)
    Console.WriteLine($"- {e.Name}");
