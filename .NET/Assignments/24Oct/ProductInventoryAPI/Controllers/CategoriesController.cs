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
    public class CategoriesController : ControllerBase
    {
        private readonly IGenericRepository<Category> _repository;
        private readonly ProductInventoryApiContext _context;

        public CategoriesController(IGenericRepository<Category> repository, ProductInventoryApiContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _repository.GetAll()
                .Select(c => new CategoryDTO
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                });
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var category = _repository.GetById(id);
            if (category == null) return NotFound();

            var dto = new CategoryDTO
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Create(CategoryDTO dto)
        {
            try
            {
                var category = new Category { CategoryName = dto.CategoryName };
                _repository.Add(category);
                _repository.Save();
                dto.CategoryId = category.CategoryId;
                return CreatedAtAction(nameof(GetById), new { id = dto.CategoryId }, dto);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database update error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CategoryDTO dto)
        {
            if (id != dto.CategoryId) return BadRequest();

            var existing = _repository.GetById(id);
            if (existing == null) return NotFound();

            existing.CategoryName = dto.CategoryName;

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

            // Check if any products exist for this category
            var hasProducts = _context.Products.Any(p => p.CategoryId == id);
            if (hasProducts)
                return BadRequest("Cannot delete category because it has products assigned.");

            try
            {
                _repository.Delete(id);
                _repository.Save();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database delete error: {ex.InnerException?.Message ?? ex.Message}");
            }

            return NoContent();
        }
    }
}