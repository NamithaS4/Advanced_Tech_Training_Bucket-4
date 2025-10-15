using Microsoft.EntityFrameworkCore;

namespace College_App.Data
{
    public class CollegeDBContext: DbContext
    {
        public CollegeDBContext(DbContextOptions<CollegeDBContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }

        public DbSet<course> Courses { get; set; }
    }
}
