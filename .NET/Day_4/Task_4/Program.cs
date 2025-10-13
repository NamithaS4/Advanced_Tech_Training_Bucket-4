using Task_4;

var orders = new List<Order>
{
    new Order{ OrderId=1001, CustomerId=1, Amount=2500, OrderDate=new DateTime(2025,5,12)},
    new Order{ OrderId=1002, CustomerId=2, Amount=1800, OrderDate=new DateTime(2025,5,13)},
    new Order{ OrderId=1003, CustomerId=1, Amount=4500, OrderDate=new DateTime(2025,5,20)},
    new Order{ OrderId=1004, CustomerId=3, Amount=6700, OrderDate=new DateTime(2025,6,01)},
    new Order{ OrderId=1005, CustomerId=4, Amount=2500, OrderDate=new DateTime(2025,6,02)},
    new Order{ OrderId=1006, CustomerId=2, Amount=5600, OrderDate=new DateTime(2025,6,10)},
    new Order{ OrderId=1007, CustomerId=5, Amount=3100, OrderDate=new DateTime(2025,6,12)},
    new Order{ OrderId=1008, CustomerId=3, Amount=7100, OrderDate=new DateTime(2025,7,01)},
    new Order{ OrderId=1009, CustomerId=4, Amount=4200, OrderDate=new DateTime(2025,7,05)},
    new Order{ OrderId=1010, CustomerId=5, Amount=2900, OrderDate=new DateTime(2025,7,10)}
};

//Finds total order amount per month.
var totalAmountPerMonth = orders.GroupBy(o => o.OrderDate.Month)
                                .Select(g => new { Month = g.Key, TotalAmount = g.Sum(o => o.Amount) });
Console.WriteLine("1. Total order amount per month:");
foreach (var item in totalAmountPerMonth)
    Console.WriteLine($"- Month {item.Month}: INR {item.TotalAmount}");

//Shows the customer who spent the most in total.
var topCustomer = orders.GroupBy(o => o.CustomerId)
                        .Select(g => new { CustomerId = g.Key, TotalSpent = g.Sum(o => o.Amount) })
                        .OrderByDescending(c => c.TotalSpent)
                        .FirstOrDefault();
Console.WriteLine("\n2. Customer who spent the most:");
if (topCustomer != null)
    Console.WriteLine($"- Customer ID {topCustomer.CustomerId} spent the most INR {topCustomer.TotalSpent}");

//Displays orders grouped by customer and show total amount spent.
var ordersByCustomer = orders.GroupBy(o => o.CustomerId)
                            .Select(g => new { CustomerId = g.Key, TotalAmount = g.Sum(o => o.Amount), Orders = g.ToList() });
Console.WriteLine("\n3. Orders grouped by customer:");
foreach (var item in ordersByCustomer)
{
    Console.WriteLine($"- Customer ID {item.CustomerId} (Total: INR {item.TotalAmount}):");
    foreach (var order in item.Orders)
        Console.WriteLine($"  - Order {order.OrderId}, Amount: INR {order.Amount}");
    Console.WriteLine();
}

//Displays the top 2 orders with the highest amount.
var top2Orders = orders.OrderByDescending(o => o.Amount).Take(2);
Console.WriteLine("\n4. Top 2 orders with the highest amount:");
foreach (var order in top2Orders)
    Console.WriteLine($"- Order {order.OrderId}, Amount: INR {order.Amount}");