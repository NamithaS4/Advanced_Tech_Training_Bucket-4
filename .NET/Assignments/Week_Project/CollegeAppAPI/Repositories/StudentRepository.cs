
using CollegeAppAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollegeAppAPI.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly CollegeDbContext _context;

        public StudentRepository(CollegeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students.Include(s => s.Course).ToListAsync();
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            return await _context.Students.Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task AddAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await GetByIdAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
    }
}
