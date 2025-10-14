using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Task_1.Model;

namespace Task_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductRepo : ControllerBase
    {
        //Task1: GET all products
        [HttpGet]
        [Route("All")]
        public ActionResult<IEnumerable<ProductDTO>> getAllProducts()
        {
            var products = ProductRepository.Products.Select(p => new ProductDTO()
            {
                ProductID = p.ProductID,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            });
            return Ok(products);
        }

        //Task2: GET product by ID
        [HttpGet("{id:Int}", Name = "getProductsByID")]
        public ActionResult<ProductDTO> getProductsByID(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var products = ProductRepository.Products.Where(n => n.ProductID == id).FirstOrDefault();
            var productDTO = new ProductDTO
            {
                ProductID = products.ProductID,
                Name = products.Name,
                Category = products.Category,
                Price = products.Price,
                StockQuantity = products.StockQuantity
            };

            if (productDTO == null)
            {
                return NotFound($"Product ID: {id} not found");
            }
            return Ok(products);
        }

        //Task3: POST → Add new product
        [HttpPost("Create")]
        public ActionResult<ProductDTO> AddProduct([FromBody] ProductDTO Model)
        {
            if (Model == null)
            {
                return BadRequest();
            }
            int productid = ProductRepository.Products.LastOrDefault().ProductID + 1;
            ProductDTO newProduct = new ProductDTO
            {
                ProductID = productid,
                Name = Model.Name,
                Category = Model.Category,
                Price = Model.Price,
                StockQuantity = Model.StockQuantity
            };
            if (newProduct.Price <= 0)
            {
                return BadRequest("Enter Valid Price for the Product");
            }
            else if (newProduct.StockQuantity <= 0)
            {
                return BadRequest("Enter Valid Stock Quantity for the Product");
            }
            ProductRepository.Products.Add(newProduct);
            return Ok(Model);
        }

        //Task4: PUT → Replace product details completely
        [HttpPut]
        public ActionResult<ProductDTO> UpdateProduct([FromBody] ProductDTO Model)
        {
            if (Model == null || Model.ProductID <= 0)
            {
                return BadRequest();
            }

            var existingProduct = ProductRepository.Products.Where(p => p.ProductID == Model.ProductID).FirstOrDefault();
            if (existingProduct == null)
            {
                return NotFound($"The product with id {Model.ProductID} not found");
            }
            existingProduct.ProductID = Model.ProductID;
            existingProduct.Name = Model.Name;
            existingProduct.Category = Model.Category;
            existingProduct.Price = Model.Price;
            existingProduct.StockQuantity = Model.StockQuantity;

            return Ok(existingProduct);
        }

        //Task5: PATCH → Update price or stock quantity only
        [HttpPatch]
        [Route("{id:int}/UpdatePartialProduct")]
        public ActionResult UpdateProductPartial(int id, [FromBody] JsonPatchDocument<ProductDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }
            var existingProduct = ProductRepository.Products.Where(p => p.ProductID == id).FirstOrDefault();
            if (existingProduct == null)
            {
                return NotFound($"The Product with id : {id} not found");
            }
            var ProductDTO = new ProductDTO()
            {
                ProductID = existingProduct.ProductID,
                Name = existingProduct.Name,
                Category = existingProduct.Category,
                Price = existingProduct.Price,
                StockQuantity = existingProduct.StockQuantity
            };
            patchDocument.ApplyTo(ProductDTO, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            existingProduct.ProductID = ProductDTO.ProductID;
            existingProduct.Name = ProductDTO.Name;
            existingProduct.Category = ProductDTO.Category;
            existingProduct.Price = ProductDTO.Price;
            existingProduct.StockQuantity = ProductDTO.StockQuantity;

            return NoContent();
        }
    }
}