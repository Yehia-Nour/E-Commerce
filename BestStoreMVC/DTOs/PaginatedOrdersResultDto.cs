using BestStoreMVC.Models;

namespace BestStoreMVC.DTOs
{
    public class PaginatedOrdersResultDto
    {
        public List<Order> Orders { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
    }
}
