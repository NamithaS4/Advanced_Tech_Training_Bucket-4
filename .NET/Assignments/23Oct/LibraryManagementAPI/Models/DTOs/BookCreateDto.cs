namespace LibraryManagementAPI.Models.DTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public int AuthorId { get; set; }
    }
}