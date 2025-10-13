using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_4
{
    internal class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public double Amount { get; set; }
        public DateTime OrderDate { get; set; }
    }
}