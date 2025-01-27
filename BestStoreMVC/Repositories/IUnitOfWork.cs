using BestStoreMVC.Models;

namespace BestStoreMVC.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Product> Products { get; }

        void Complete();
    }
}
