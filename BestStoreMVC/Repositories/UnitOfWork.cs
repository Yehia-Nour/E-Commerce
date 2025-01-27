using BestStoreMVC.Models;
using BestStoreMVC.Services;

namespace BestStoreMVC.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IGenericRepository<Product> Products { get; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new GenericRepository<Product>(_context);
        }

        public void Complete()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}