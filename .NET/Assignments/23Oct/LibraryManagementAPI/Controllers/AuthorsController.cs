using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _repository;

        public AuthorsController(IAuthorRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _repository.GetAllAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await _repository.GetByIdAsync(id);
            if (author == null) return NotFound();
            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AuthorCreateDto dto)
        {
            var author = new Author
            {
                Name = dto.Name,
                Country = dto.Country
            };

            var created = await _repository.AddAsync(author);
            return CreatedAtAction(nameof(GetById), new { id = created.AuthorID }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AuthorCreateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            existing.Country = dto.Country;

            await _repository.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}