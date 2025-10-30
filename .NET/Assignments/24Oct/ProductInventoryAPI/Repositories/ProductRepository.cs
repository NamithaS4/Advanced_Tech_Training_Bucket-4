using Microsoft.EntityFrameworkCore;
using ProductInventoryAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductInventoryAPI.Repository
{
    public class ProductRepository : GenericRepository<Product>
    {
        public ProductRepository(ProductInventoryApiContext context) : base(context) { }

        public IEnumerable<Product> GetAllWithCategory()
        {
            return _context.Products.Include(p => p.Category).ToList();
        }

        public Product? GetByIdWithCategory(int id)
        {
            return _context.Products.Include(p => p.Category)
                                    .FirstOrDefault(p => p.ProductId == id);
        }
    }
}