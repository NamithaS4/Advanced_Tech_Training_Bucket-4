using Task_3;

var students = new List<Student>
{
    new Student{ Id=1, Name="Asha", Course="C#", Marks=92, City="Bangalore"},
    new Student { Id = 2, Name = "Ravi", Course = "Java", Marks = 85, City = "Pune" },
    new Student { Id = 3, Name = "Sneha", Course = "Python", Marks = 78, City = "Hyderabad" },
    new Student { Id = 4, Name = "Kiran", Course = "C#", Marks = 88, City = "Delhi" },
    new Student { Id = 5, Name = "Meena", Course = "Python", Marks = 95, City = "Bangalore" },
    new Student { Id = 6, Name = "Vijay", Course = "C#", Marks = 82, City = "Chennai" },
    new Student { Id = 7, Name = "Deepa", Course = "Java", Marks = 91, City = "Mumbai" },
    new Student { Id = 8, Name = "Arjun", Course = "Python", Marks = 89, City = "Hyderabad" },
    new Student { Id = 9, Name = "Priya", Course = "C#", Marks = 97, City = "Pune" },
    new Student { Id = 10, Name = "Rohit", Course = "Java", Marks = 74, City = "Delhi" }
};

//Finds the highest scorer in each course.
var highestScorerByCourse = students.GroupBy(s => s.Course)
                                    .Select(g => new { Course = g.Key, TopStudent = g.OrderByDescending(s => s.Marks).First() });
Console.WriteLine("1. Highest scorer in each course:");
foreach (var item in highestScorerByCourse)
    Console.WriteLine($"- {item.Course}: {item.TopStudent.Name}");

//Displays average marks of all students city-wise.
var averageMarksByCity = students.GroupBy(s => s.City)
                                 .Select(g => new { City = g.Key, AverageMarks = g.Average(s => s.Marks) });
Console.WriteLine("\n2. Average marks city-wise:");
foreach (var item in averageMarksByCity)
    Console.WriteLine($"- {item.City}: {item.AverageMarks:F2}");

//Displays names and marks of students ranked by marks.
var studentsRankedByMarks = students.OrderByDescending(s => s.Marks);
Console.WriteLine("\n3. Students ranked by marks:");
foreach (var s in studentsRankedByMarks)
    Console.WriteLine($"- {s.Name}: {s.Marks} marks");