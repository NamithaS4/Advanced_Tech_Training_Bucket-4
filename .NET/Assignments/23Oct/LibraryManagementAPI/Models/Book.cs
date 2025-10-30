using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryManagementAPI.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Genre { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        [JsonIgnore]
        public Author? Author { get; set; }
    }
}