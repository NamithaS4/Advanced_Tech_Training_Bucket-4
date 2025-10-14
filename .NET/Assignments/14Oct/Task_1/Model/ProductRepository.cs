namespace Task_1.Model
{
    public class ProductRepository
    {
        public static List<ProductDTO> Products { get; set; } = new List<ProductDTO>
        {
            new ProductDTO
            {
                ProductID = 1,
                Name = "Apple iPhone 15",
                Category = "Electronics",
                Price = 999.99m,
                StockQuantity = 50
            },
            new ProductDTO
            {
                ProductID = 2,
                Name = "Samsung Galaxy S23",
                Category = "Electronics",
                Price = 899.99m,
                StockQuantity = 35
            },
            new ProductDTO
            {
                ProductID = 3,
                Name = "Nike Running Shoes",
                Category = "Footwear",
                Price = 120.50m,
                StockQuantity = 100
            },
            new ProductDTO
            {
                ProductID = 4,
                Name = "Sony WH-1000XM5 Headphones",
                Category = "Electronics",
                Price = 350.00m,
                StockQuantity = 20
            },
            new ProductDTO
            {
                ProductID = 5,
                Name = "Levi's Denim Jeans",
                Category = "Clothing",
                Price = 60.00m,
                StockQuantity = 75
            }
        };
    }
}