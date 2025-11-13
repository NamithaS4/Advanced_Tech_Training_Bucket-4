using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Kchbhi.Model;

namespace Kchbhi.Data
{
    public class KchbhiContext : DbContext
    {
        public KchbhiContext (DbContextOptions<KchbhiContext> options)
            : base(options)
        {
        }

        public DbSet<Kchbhi.Model.Student> Student { get; set; } = default!;
    }
}
