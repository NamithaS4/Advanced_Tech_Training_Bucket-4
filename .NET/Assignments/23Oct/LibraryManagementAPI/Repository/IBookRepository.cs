using LibraryManagementAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagementAPI.Repository
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(int id);
        Task<Book> AddAsync(Book book);
        Task<Book> UpdateAsync(Book book);
        Task<bool> DeleteAsync(int id);
        Task<int?> GetAuthorIdByBookIdAsync(int bookId);
    }
}