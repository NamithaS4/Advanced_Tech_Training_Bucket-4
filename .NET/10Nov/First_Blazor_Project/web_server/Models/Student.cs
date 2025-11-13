using System;
using System.Collections.Generic;

namespace web_server.Models;

public partial class Student
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Age { get; set; }

    public DateTime? Birthday { get; set; }
}
