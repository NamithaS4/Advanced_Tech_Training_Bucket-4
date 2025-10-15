using College_App.Model.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace College_App.Data
{
    public class Student
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int studentId { get; set; }
        
        public string name { get; set; }
   

        public int age { get; set; }
        
        public string email { get; set; }
    
        public string password { get; set; }
        
        public string confirmPassword { get; set; }

        public DateTime Admission { get; set; } = DateTime.Now;
    }
}
