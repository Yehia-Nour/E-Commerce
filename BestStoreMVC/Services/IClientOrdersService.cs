using BestStoreMVC.Models;

namespace BestStoreMVC.Services
{
    public interface IClientOrdersService
    {
        Task<List<Order>> GetOrdersAsync(string clientId, int pageIndex, int pageSize);
        Task<int> GetTotalPagesAsync(string clientId, int pageSize);
        Task<Order?> GetOrderDetailsAsync(string clientId, int orderId);
    }
}
