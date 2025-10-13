namespace Task_2
{
    //Movie Ticket Booking System

    class Movie
    {
        public string movieName { get; set; }
        public int totalSeats { get; set; }
        public int bookedSeats { get; set; }
        public Movie(string movieName, int totalSeats, int bookedSeats)
        {
            this.movieName = movieName;
            this.totalSeats = totalSeats;
            this.bookedSeats = bookedSeats;
        }
        public void BookSeats(int numberOfSeats)
        {
            if(numberOfSeats <= (totalSeats - bookedSeats))
            {
                bookedSeats += numberOfSeats;
                Console.WriteLine($"{numberOfSeats} seats booked successfully for {movieName}.");
            }
            else
            {
                Console.WriteLine($"Only {totalSeats - bookedSeats} seats are available for {movieName}.");
            }
        }

        public int CancelSeats(int numOfSeats)
        {
            if(numOfSeats <= bookedSeats)
            {
                bookedSeats -= numOfSeats;
                Console.WriteLine($"{numOfSeats} seats cancelled successfully for {movieName}.");
            }
            else
            {
                Console.WriteLine($"Only {bookedSeats} seats are booked for {movieName}.");
            }
            return bookedSeats;

        }

        public void DisplayAvailableSeats()
        {
            Console.WriteLine($"Available seats for {movieName}: {totalSeats - bookedSeats}");
        }

    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Movie movie1 = new Movie("Sinchan", 100, 70);
            Movie movie2 = new Movie("Black Clover", 150, 130);
            Movie movie3 = new Movie("Interstellar", 200, 100);
            Movie movie4 = new Movie("Avatar", 120, 80);


            Console.WriteLine("\nWelcome to the Movie Ticket Booking System\n");
            movie1.DisplayAvailableSeats();
            movie2.DisplayAvailableSeats();
            movie3.DisplayAvailableSeats();
            movie4.DisplayAvailableSeats();


            movie1.BookSeats(20);
            movie2.BookSeats(30);
            movie3.BookSeats(50);
            movie4.BookSeats(50);

           movie1.DisplayAvailableSeats();
           movie2.DisplayAvailableSeats();
           movie3.DisplayAvailableSeats();
           movie4.DisplayAvailableSeats();

            movie1.CancelSeats(10);
            movie2.CancelSeats(20);
             
            movie1.DisplayAvailableSeats();
            movie2.DisplayAvailableSeats();
            movie3.DisplayAvailableSeats();
            movie4.DisplayAvailableSeats();


        }
    }
}
