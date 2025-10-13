using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_4
{
    internal class Games
    {
        public required string Title { get; set; }
        public string? Genre { get; set; }
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }
        public decimal Price { get; set; }
    }
}
