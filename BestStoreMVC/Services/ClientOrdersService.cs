using BestStoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Services
{
    public class ClientOrdersService : IClientOrdersService
    {
        private readonly ApplicationDbContext _context;

        public ClientOrdersService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersAsync(string clientId, int pageIndex, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.ClientId == clientId)
                .OrderByDescending(o => o.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalPagesAsync(string clientId, int pageSize)
        {
            int count = await _context.Orders.CountAsync(o => o.ClientId == clientId);
            return (int)Math.Ceiling((double)count / pageSize);
        }

        public async Task<Order?> GetOrderDetailsAsync(string clientId, int orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.ClientId == clientId)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
