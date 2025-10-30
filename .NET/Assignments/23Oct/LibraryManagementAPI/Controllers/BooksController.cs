using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;
        private readonly IAuthorRepository _authorRepository;

        public BooksController(IBookRepository repository, IAuthorRepository authorRepository)
        {
            _repository = repository;
            _authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _repository.GetAllAsync();

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _repository.GetByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpGet("{bookId}/author")]
        public async Task<IActionResult> GetAuthorOfBook(int bookId)
        {
            var authorId = await _repository.GetAuthorIdByBookIdAsync(bookId);
            if (authorId == null) return NotFound();

            var author = await _authorRepository.GetByIdAsync(authorId.Value);
            if (author == null) return NotFound();

            return Ok(author);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookCreateDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                Genre = dto.Genre,
                AuthorId = dto.AuthorId
            };

            var created = await _repository.AddAsync(book);
            return CreatedAtAction(nameof(GetById), new { id = created.BookId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookCreateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Title = dto.Title;
            existing.Genre = dto.Genre;
            existing.AuthorId = dto.AuthorId;

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