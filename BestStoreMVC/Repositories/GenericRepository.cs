using BestStoreMVC.Services;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }


        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Create(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(int id)
        {
            _dbSet.Remove(_dbSet.Find(id));
        }
    }
}
