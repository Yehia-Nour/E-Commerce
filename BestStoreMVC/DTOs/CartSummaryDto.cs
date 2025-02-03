using BestStoreMVC.Models;

namespace BestStoreMVC.DTOs
{
    public class CartSummaryDto
    {
            public List<OrderItem> CartItems { get; set; } = new();
            public decimal Total { get; set; }
            public int CartSize { get; set; }
            public string DeliveryAddress { get; set; } = string.Empty;
            public string PaymentMethod { get; set; } = string.Empty;
    }
}
