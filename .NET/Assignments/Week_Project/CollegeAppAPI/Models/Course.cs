using System;
using System.Collections.Generic;

namespace CollegeAppAPI.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseCode { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string? Department { get; set; }

    public int? Semester { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
