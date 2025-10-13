using System.ComponentModel.DataAnnotations;

namespace College_App.Model
{
    public class Student
    {
        [Required]

        public int studentId { get; set; }
        
        public string name { get; set; }

        public int age { get; set; }
        public string email { get; set; }
    }
}
