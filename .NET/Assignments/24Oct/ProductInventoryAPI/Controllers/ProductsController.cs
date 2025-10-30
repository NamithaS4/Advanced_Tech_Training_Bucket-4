using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductInventoryAPI.DTOs;
using ProductInventoryAPI.Models;
using ProductInventoryAPI.Repository;

using System.Linq;

namespace ProductInventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _repository;
        private readonly ProductInventoryApiContext _context;

        public ProductsController(ProductRepository repository, ProductInventoryApiContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _repository.GetAllWithCategory()
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.CategoryName
                });

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _repository.GetByIdWithCategory(id);
            if (product == null) return NotFound();

            var dto = new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName
            };
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Create(ProductDTO dto)
        {
            try
            {
                // Check if category exists
                var categoryExists = _context.Categories.Any(c => c.CategoryId == dto.CategoryId);
                if (!categoryExists)
                    return BadRequest($"Category with ID {dto.CategoryId} does not exist.");

                var product = new Product
                {
                    ProductName = dto.ProductName,
                    Price = dto.Price,
                    StockQuantity = dto.StockQuantity,
                    CategoryId = dto.CategoryId
                };

                _repository.Add(product);
                _repository.Save();
                dto.ProductId = product.ProductId;

                return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, dto);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database update error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductDTO dto)
        {
            if (id != dto.ProductId) return BadRequest();

            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();

            // Check if category exists
            var categoryExists = _context.Categories.Any(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                return BadRequest($"Category with ID {dto.CategoryId} does not exist.");

            existing.ProductName = dto.ProductName;
            existing.Price = dto.Price;
            existing.StockQuantity = dto.StockQuantity;
            existing.CategoryId = dto.CategoryId;

            try
            {
                _repository.Update(existing);
                _repository.Save();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database update error: {ex.InnerException?.Message ?? ex.Message}");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();

            try
            {
                _repository.Delete(id);
                _repository.Save();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Cannot delete product: {ex.InnerException?.Message ?? ex.Message}");
            }

            return NoContent();
        }
    }
}