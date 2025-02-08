using BestStoreMVC.DTOs;
using BestStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Services
{
    public class AdminOrdersService : IAdminOrdersService
    {
        private readonly ApplicationDbContext _context;
        private readonly int _pageSize = 5;

        public AdminOrdersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaginatedOrdersResultDto GetAllOrders(int pageIndex)
        {
            var query = _context.Orders.Include(o => o.Client)
                .Include(o => o.Items)
                .OrderByDescending(o => o.Id)
                .AsQueryable();

            var (paginatedItems, totalPages) = ApplyPagination(query, pageIndex);

            return new PaginatedOrdersResultDto
            {
                Orders = paginatedItems,
                PageIndex = pageIndex,
                TotalPages = totalPages
            };
        }

        public Order? GetOrderById(int id)
        {
            return _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id);
        }

        public int GetClientOrderCount(string clientId)
        {
            return _context.Orders.Count(o => o.ClientId == clientId);
        }

        public bool UpdateOrderStatus(int id, string? paymentStatus, string? orderStatus)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return false;

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                order.PaymentStatus = paymentStatus;
            }

            if (!string.IsNullOrEmpty(orderStatus))
            {
                order.OrderStatus = orderStatus;
            }

            _context.SaveChanges();
            return true;
        }

        private (List<Order> PaginatedItems, int TotalPages) ApplyPagination(IQueryable<Order> query, int pageIndex)
        {
            if (pageIndex < 1) pageIndex = 1;

            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling(totalCount / (double)_pageSize);
            var paginatedItems = query.Skip((pageIndex - 1) * _pageSize).Take(_pageSize).ToList();

            return (paginatedItems, totalPages);
        }
    }
}
