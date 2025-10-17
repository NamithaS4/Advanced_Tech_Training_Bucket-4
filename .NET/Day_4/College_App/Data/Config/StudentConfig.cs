using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace College_App.Data.Config
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Student");
            builder.HasKey(e => e.studentId);
            builder.Property(e => e.studentId).UseIdentityColumn();

            builder.Property(e => e.name).IsRequired().HasMaxLength(250);
            builder.Property(e => e.email).IsRequired();
            builder.Property(e => e.age).IsRequired();

            builder.HasData(new List<Student>
                        {
                            new Student {
                                studentId = 1,
                                name = "Nami",
                                email = "123@gmail.com",
                                age = 20
                                 }
                        });

        }
    }
}
