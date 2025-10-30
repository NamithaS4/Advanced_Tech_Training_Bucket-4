namespace ProductInventoryAPI.DTOs
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; } // for output
    }
}