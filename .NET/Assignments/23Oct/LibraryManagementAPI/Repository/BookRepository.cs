using LibraryManagementAPI.Data;
using LibraryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementAPI.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == id);
        }
        public async Task<int?> GetAuthorIdByBookIdAsync(int bookId)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            return book?.AuthorId;
        }

        public async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateAsync(Book book)
        {
            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}