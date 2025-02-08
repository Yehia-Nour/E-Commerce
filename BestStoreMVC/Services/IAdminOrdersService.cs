using BestStoreMVC.DTOs;
using BestStoreMVC.Models;

namespace BestStoreMVC.Services
{
    public interface IAdminOrdersService
    {
        PaginatedOrdersResultDto GetAllOrders(int pageIndex);
        Order? GetOrderById(int id);
        int GetClientOrderCount(string clientId);
        bool UpdateOrderStatus(int id, string? paymentStatus, string? orderStatus);
    }
}
