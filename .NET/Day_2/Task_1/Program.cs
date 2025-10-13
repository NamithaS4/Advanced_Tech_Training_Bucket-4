namespace Task_1
{
    class Book
    {
        //Library Management System
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool isIssued { get; set; }
        
        public Book(int bookId, string title, string author)
        {
            this.BookId = bookId;
            this.Title = title;
            this.Author = author;
            this.isIssued = false;

        }

        public void IssueBook()
        {
            if (!isIssued)
            {
                isIssued = true;
                Console.WriteLine($"Book '{Title}' issued successfully.");
            }
            else
            {
                Console.WriteLine($"Book '{Title}' is already issued.");
            }
        }

        public void ReturnBook()
        {
            if (isIssued)
            {
                isIssued = false;
                Console.WriteLine($"Book '{Title}' returned successfully.");
            }
            else
            {
                Console.WriteLine($"Book '{Title}' was not issued.");
            }
        }

        public void DisplayBookDetails()
        {
            Console.WriteLine($"Book ID: {BookId}\nTitle: {Title}\nAuthor: {Author}\nIssued: {isIssued}\n");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Book book1 = new Book(1,"Naruto", "Masashi Kishimoto");
            Book book2 = new Book(2,"Bleach", "Noriaki Kubo");
            Book book3 = new Book(3,"One Piece", "Eiichiro Oda");
            Book book4 = new Book(4,"Black Clover", "Yūki Tabata");

            Console.WriteLine("\nInitial Book Details:\n");
            book1.DisplayBookDetails();
            book2.DisplayBookDetails();
            book3.DisplayBookDetails();
            book4.DisplayBookDetails();

            Console.WriteLine("\nIssuing and Returning book details:\n");

            book1.IssueBook();
            book2.IssueBook();
            book3.IssueBook();
            book4.IssueBook();

            book2.ReturnBook();
            book3.ReturnBook();

            Console.WriteLine("\nFinal Book Details:\n");
            book1.DisplayBookDetails();
            book2.DisplayBookDetails();
            book3.DisplayBookDetails();
            book4.DisplayBookDetails();

            Console.ReadLine();
        }
    }
}
