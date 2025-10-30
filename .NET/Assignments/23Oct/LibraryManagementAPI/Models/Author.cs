using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace LibraryManagementAPI.Models
{
    public class Author
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Country { get; set; }

    }
}