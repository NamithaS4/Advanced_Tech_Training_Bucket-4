using College_App.Data.Config;
using Microsoft.EntityFrameworkCore;

namespace College_App.Data
{
    public class CollegeDBContext: DbContext
    {
        public CollegeDBContext(DbContextOptions<CollegeDBContext> options) : base(options)
        {

        }
        public DbSet<Student> Student { get; set; }

        public DbSet<course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentConfig());
        }
    }

}

