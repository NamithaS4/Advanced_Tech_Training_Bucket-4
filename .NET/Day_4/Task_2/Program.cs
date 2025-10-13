
using Task_2;

var products = new List<Product>
{
    new Product{ Id=1, Name="Laptop", Category="Electronics", Price=75000, Stock=15 },
    new Product{ Id=2, Name="Smartphone", Category="Electronics", Price=55000, Stock=25 },
    new Product{ Id=3, Name="Tablet", Category="Electronics", Price=30000, Stock=10 },
    new Product{ Id=4, Name="Headphones", Category="Accessories", Price=2000, Stock=100 },
    new Product{ Id=5, Name="Shirt", Category="Fashion", Price=1500, Stock=50 },
    new Product{ Id=6, Name="Jeans", Category="Fashion", Price=2200, Stock=30 },
    new Product{ Id=7, Name="Shoes", Category="Fashion", Price=3500, Stock=20 },
    new Product{ Id=8, Name="Refrigerator", Category="Appliances", Price=45000, Stock=8 },
    new Product{ Id=9, Name="Washing Machine", Category="Appliances", Price=38000, Stock=6 },
    new Product{ Id=10, Name="Microwave", Category="Appliances", Price=12000, Stock=12 }
};

//Displays all products with stock less than 20.
var lowStockProducts = products.Where(p => p.Stock < 20);
Console.WriteLine("1. Products with low stock (< 20):");
foreach (var p in lowStockProducts)
    Console.WriteLine($"- {p.Name}");

// Shows all products belonging to the "Fashion" category.
var fashionProducts = products.Where(p => p.Category == "Fashion");
Console.WriteLine("\n2. Products in 'Fashion' category:");
foreach (var p in fashionProducts)
    Console.WriteLine($"- {p.Name}");

// 3. Display product names and prices where price is greater than 10,000.
var expensiveProducts = products.Where(p => p.Price > 10000);
Console.WriteLine("\n3. Products with price more than 10,000:");
foreach (var p in expensiveProducts)
    Console.WriteLine($"- {p.Name}: INR {p.Price}");

//List of all product names sorted by price (descending).
var sortedByPriceDesc = products.OrderByDescending(p => p.Price);
Console.WriteLine("\n4. Products sorted by price (descending):");
foreach (var p in sortedByPriceDesc)
    Console.WriteLine($"- {p.Name}: INR {p.Price}");

// Finds the most expensive product in each category.
var mostExpensiveByCategory = products.GroupBy(p => p.Category)
                                        .Select(g => new { Category = g.Key, Product = g.OrderByDescending(p => p.Price).First() });
Console.WriteLine("\n5. Most expensive product per category:");
foreach (var item in mostExpensiveByCategory)
    Console.WriteLine($"- {item.Category}: {item.Product.Name}");

//Shows total stock per category.
var totalStockByCategory = products.GroupBy(p => p.Category)
                                    .Select(g => new { Category = g.Key, TotalStock = g.Sum(p => p.Stock) });
Console.WriteLine("\n6. Total stock per category:");
foreach (var item in totalStockByCategory)
    Console.WriteLine($"- {item.Category}: {item.TotalStock}");

//Displays products whose name starts with ‘S’.
var productsStartingWithS = products.Where(p => p.Name.StartsWith("S"));
Console.WriteLine("\n7. Products starting with 'S':");
foreach (var p in productsStartingWithS)
    Console.WriteLine($"- {p.Name}");

// 8. Show average price of products in each category.
var averagePriceByCategory = products.GroupBy(p => p.Category)
                                    .Select(g => new { Category = g.Key, AveragePrice = Math.Round(g.Average(p => p.Price), 2) });
Console.WriteLine("\n8. Average price per category:");
foreach (var item in averagePriceByCategory)
    Console.WriteLine($"- {item.Category}: INR {item.AveragePrice}");
