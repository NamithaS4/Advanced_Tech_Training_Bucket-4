using System.ComponentModel.DataAnnotations;

namespace Task_1.Model
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}