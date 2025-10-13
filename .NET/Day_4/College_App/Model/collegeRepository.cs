namespace College_App.Model
{
    public class collegeRepository
    {
        public static List<Student> students { get; set; } = new List<Student>(){ new Student
       {
           studentId = 1,
           name = "Test",
           age = 1,
           email = "shivam@gmail.com"
       },

       new Student {

           studentId = 2,
           name = "Tester",
           age = 2,
           email = "dsckjbdsc"
       }

   };
    }
}
