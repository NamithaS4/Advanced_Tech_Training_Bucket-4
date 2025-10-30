using LibraryManagementAPI.Data;
using LibraryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementAPI.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryDbContext _context;

        public AuthorRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _context.Authors
                .ToListAsync();
        }

        public async Task<Author?> GetByIdAsync(int id)
        {
            return await _context.Authors
                .FirstOrDefaultAsync(a => a.AuthorID == id);
        }

        public async Task<Author> AddAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<Author> UpdateAsync(Author author)
        {
            _context.Entry(author).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return false;
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}