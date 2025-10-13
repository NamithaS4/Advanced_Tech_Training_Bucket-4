using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using College_App.Model.Validators;

namespace College_App.Model
{
    public class studentDTO
    {
        [ValidateNever]
        public int studentId { get; set; }
        [Required(ErrorMessage = "Please Enter your Name")]
        [StringLength(100)]
        [Capitalize]
        public string name { get; set; }
        [Range(10, 30)]

        public int age { get; set; }
        [EmailAddress(ErrorMessage = "Please Enter a your email")]
        public string email { get; set; }
        [SpaceCheck]
        public string password { get; set; }
        [Compare(nameof(password))]
        public string confirmPassword { get; set; }

        public DateTime Admission { get; set; } = DateTime.Now;
    }

    
}
