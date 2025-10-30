using Microsoft.EntityFrameworkCore;
using ProductInventoryAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductInventoryAPI.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ProductInventoryApiContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ProductInventoryApiContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAll() => _dbSet.ToList();

        public T? GetById(int id) => _dbSet.Find(id);

        public void Add(T entity) => _dbSet.Add(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
                _dbSet.Remove(entity);
        }

        public void Save() => _context.SaveChanges();
    }
}