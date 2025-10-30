using ProductInventoryAPI.Models;
using ProductInventoryAPI.Repository;



namespace ProductInventoryAPI.Repository
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository(ProductInventoryApiContext context) : base(context)
        {
        }
    }
}